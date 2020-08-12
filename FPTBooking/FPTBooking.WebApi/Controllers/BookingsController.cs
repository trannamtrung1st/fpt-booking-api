using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using FPTBooking.Business;
using FPTBooking.Business.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.General;
using FPTBooking.Business.Services;
using FPTBooking.Data;
using FPTBooking.Business.Helpers;
using FirebaseAdmin.Messaging;
using FPTBooking.Business.Queries;
using FPTBooking.Data.Models;

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.BOOKING_API)]
    [ApiController]
    [InjectionFilter]
    public class BookingsController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly RoomBusinessService _roomService;
        [Inject]
        private readonly BookingBusinessService _service;
        [Inject]
        private readonly SystemService _sysService;
        [Inject]
        private readonly MemberService _memberService;

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetOwnerBookings([FromQuery][QueryObject]BookingQueryFilter filter,
            [FromQuery]BookingQuerySort sort,
            [FromQuery]BookingQueryProjection projection,
            [FromQuery]BookingQueryPaging paging,
            [FromQuery]BookingQueryOptions options)
        {
            var validationData = _service.ValidateGetBookings(
                User, BookingPrincipalRelationship.Owner, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryBookingDynamic(
                User, null, BookingPrincipalRelationship.Owner,
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

        [Authorize]
        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendar(
            [FromQuery][DefaultDateTimeModelBinder]DateTime date,
            [FromQuery]BookingQueryProjection projection,
            [FromQuery]BookingQueryOptions options)
        {
            var validationData = _service.ValidateGetOwnerBookings(
                User, date, projection, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var member = _memberService.Members.Id(UserId).FirstOrDefault();
            var result = await _service.QueryCalendarDynamic(User, member, projection,
                date, validationData.TempData, options);
            return Ok(AppResult.Success(data: result));
        }

#if !DEBUG
        [Authorize(Roles = RoleName.MANAGER)]
#endif
        [HttpGet("managed")]
        public async Task<IActionResult> GetManagedRequests([FromQuery][QueryObject]BookingQueryFilter filter,
            [FromQuery]BookingQuerySort sort,
            [FromQuery]BookingQueryProjection projection,
            [FromQuery]BookingQueryPaging paging,
            [FromQuery]BookingQueryOptions options)
        {
            var validationData = _service.ValidateGetBookings(
                User, BookingPrincipalRelationship.Manager, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var member = _memberService.Members.Id(UserId).FirstOrDefault();
            var result = await _service.QueryBookingDynamic(
                User, member, BookingPrincipalRelationship.Manager,
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> Create(CreateBookingModel model)
        {
            var memberQuery = _memberService.Members;
            var member = _memberService.Members.Id(UserId).FirstOrDefault();
            var validationData = _service.ValidateCreateBooking(User, member, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var usingMemberIds = validationData.GetTempData<List<string>>("using_member_ids");
            var bookedRoom = validationData.GetTempData<Room>("booked_room");
            AppEvent ev; Booking entity;
            using (var trans = context.Database.BeginTransaction())
            {
                entity = _service.CreateBooking(member, bookedRoom, model, usingMemberIds);
                var history = _service.CreateHistoryForCreateBooking(entity, member);
                _roomService.ChangeRoomHangingStatus(bookedRoom, false, UserId);
                //log event
                ev = _sysService.GetEventForBookingProcessing(history);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            //notify using members, managers (if any)
            var notiData = new Dictionary<string, string>
            {
                { "event", ev.Type },
                { "id", entity.Id.ToString() }
            };
            var notiMemberIds = usingMemberIds.Where(o => o != UserId).ToList();
            var notiMembers = notiMemberIds.Any() ? NotiHelper.Notify(notiMemberIds, new Notification
            {
                Title = $"You have a new booking",
                Body = $"{UserEmail} has just created a booking of room {entity.RoomCode} for you. Press for more detail"
            }, data: notiData) : Task.CompletedTask;
            var managerNoti = new Notification
            {
                Title = $"There's a new booking request",
                Body = $"{UserEmail} has just created a booking of room {entity.RoomCode}. Press for more detail"
            };
            if (entity.Status == BookingStatusValues.PROCESSING)
            {
                var managerIds = _memberService.QueryManagersOfMember(member.UserId)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Count > 0)
                    await NotiHelper.Notify(managerIds, managerNoti, data: notiData);
            }
            else if (entity.Status == BookingStatusValues.VALID)
            {
                var managerIds = _memberService.QueryManagersOfArea(bookedRoom.BuildingAreaCode)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Count > 0)
                    await NotiHelper.Notify(managerIds, managerNoti, data: notiData);
            }
            await notiMembers;
            return Created($"/{ApiEndpoint.BOOKING_API}/{entity.Id}",
                AppResult.Success(data: entity.Id));
        }

        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id, CancelBookingModel model)
        {
            var entity = _service.Bookings.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateCancelBooking(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var fromStatus = entity.Status;
            AppEvent ev;
            using (var trans = context.Database.BeginTransaction())
            {
                _service.CancelBooking(model, entity);
                var history = _service.CreateHistoryForCancelBooking(entity, fromStatus, entity.BookMember);
                //log event
                ev = _sysService.GetEventForBookingProcessing(history);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            //notify using members, managers (if any)
            var notiData = new Dictionary<string, string>
            {
                { "event", ev.Type },
                { "id", entity.Id.ToString() }
            };
            var notiMemberIds = entity.UsingMemberIds.Split('\n')
                .Where(o => o != UserId).ToList();
            var notiMembers = notiMemberIds.Any() ? NotiHelper.Notify(notiMemberIds, new Notification
            {
                Title = $"Your booking {entity.Code} has been aborted",
                Body = $"{UserEmail} has just aborted booking {entity.Code}. Press for more detail"
            }, data: notiData) : Task.CompletedTask;
            var managerNoti = new Notification
            {
                Title = $"A booking managed by you has been aborted",
                Body = $"{UserEmail} has just aborted booking {entity.Code}. Press for more detail"
            };
            if (entity.Status == BookingStatusValues.VALID)
            {
                var managerIds = _memberService.QueryManagersOfMember(entity.BookMemberId)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Count > 0)
                    await NotiHelper.Notify(managerIds, managerNoti, data: notiData);
            }
            else if (entity.Status == BookingStatusValues.APPROVED)
            {
                var managerIds = _memberService.QueryManagersOfArea(entity.Room.BuildingAreaCode)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Count > 0)
                    await NotiHelper.Notify(managerIds, managerNoti, data: notiData);
            }
            await notiMembers;
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/feedback")]
        public IActionResult FeedbackBooking(int id, FeedbackBookingModel model)
        {
            var entity = _service.Bookings.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateFeedbackBooking(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var fromStatus = entity.Status;
            using (var trans = context.Database.BeginTransaction())
            {
                _service.FeedbackBooking(model, entity);
                var history = _service.CreateHistoryForFeedbackBooking(entity, entity.BookMember);
                //log event
                var ev = _sysService.GetEventForBookingProcessing(history);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            //notify managers (if any)
            //var managerIds = _memberService.QueryManagersOfArea(entity.Room.BuildingAreaCode)
            //    .Select(o => o.UserId).ToList();
            //if (managerIds.Count > 0)
            //    await NotiHelper.Notify(managerIds, managerNoti);
            return NoContent();
        }

        [Authorize(Roles = RoleName.MANAGER)]
        [HttpPost("{id}/approval")]
        public async Task<IActionResult> ChangeApproveStatusOfBooking(int id, ChangeApprovalStatusOfBookingModel model)
        {
            var entity = _service.Bookings.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var member = _memberService.Members.Id(UserId).FirstOrDefault();
            var validationData = _service.ValidateChangeApprovalStausOfBooking(User, member, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var fromStatus = entity.Status;
            AppEvent ev;
            using (var trans = context.Database.BeginTransaction())
            {
                _service.ChangeApprovalStatusOfBooking(model, entity);
                var history = _service.CreateHistoryForChangeApprovalStatusOfBooking(
                    entity, fromStatus, model.IsApproved, member);
                //log event
                ev = _sysService.GetEventForBookingProcessing(history);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            //notify using members, managers (if any)
            var notiData = new Dictionary<string, string>
            {
                { "event", ev.Type },
                { "id", entity.Id.ToString() }
            };
            var approvePerson = fromStatus == BookingStatusValues.PROCESSING ? "department manager" :
                "location manager";
            var action = model.IsApproved ? "approved" : "denied";
            var usingMemberIds = entity.UsingMemberIds.Split('\n');
            var notiMemberIds = usingMemberIds.Where(o => o != UserId).ToList();
            var notiMembers = NotiHelper.Notify(notiMemberIds, new Notification
            {
                Title = $"Booking {entity.Code} has been {action} by your {approvePerson}",
                Body = $"{UserEmail} has just {action} your booking. Press for more detail"
            }, data: notiData);
            if (entity.Status == BookingStatusValues.VALID && model.IsApproved)
            {
                var managerIds = _memberService.QueryManagersOfArea(entity.Room.BuildingAreaCode)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Count > 0)
                    await NotiHelper.Notify(managerIds, new Notification
                    {
                        Title = $"There's a new booking request",
                        Body = $"{UserEmail} has just {action} a booking of room {entity.RoomCode}. Press for more detail"
                    }, data: notiData);
            }
            await notiMembers;
            return NoContent();
        }

        [Authorize(Roles = RoleName.MANAGER)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingModel model)
        {
            var entity = _service.Bookings.Id(id).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var member = _memberService.Members.Id(UserId).FirstOrDefault();
            var validationData = _service.ValidateUpdateBooking(User, member, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var fromStatus = entity.Status;
            AppEvent ev;
            using (var trans = context.Database.BeginTransaction())
            {
                _service.UpdateBooking(model, entity);
                var history = _service.CreateHistoryForUpdateBooking(entity, member);
                //log event
                ev = _sysService.GetEventForBookingProcessing(history);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            //notify using members
            var notiData = new Dictionary<string, string>
            {
                { "event", ev.Type },
                { "id", entity.Id.ToString() }
            };
            var updatePerson = fromStatus == BookingStatusValues.PROCESSING ? "department manager" :
                "location manager";
            var usingMemberIds = entity.UsingMemberIds.Split('\n');
            var notiMemberIds = usingMemberIds.Where(o => o != UserId).ToList();
            await NotiHelper.Notify(notiMemberIds, new Notification
            {
                Title = $"Booking {entity.Code} has been updated by your {updatePerson}",
                Body = $"{UserEmail} has just updated your booking of room {entity.RoomCode}. Press for more detail"
            }, data: notiData);
            return NoContent();
        }

#if !DEBUG
        [Authorize]
#endif
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id,
            [FromQuery]BookingQueryProjection projection,
            [FromQuery]BookingQueryOptions options)
        {
            var getManagerType = projection.GetFieldsArr().Contains(BookingQueryProjection.MANAGER_TYPE);
            projection = new BookingQueryProjection { fields = BookingQueryProjection.DETAIL };
            var entity = _service.GetBookingDetail(id, projection);
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateGetBookingDetail(entity, User, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var obj = _service.GetBookingDynamic(entity, projection, options);
            if (getManagerType)
                obj["manager_type"] = validationData.TempData["manager_type"];
            return Ok(AppResult.Success(data: obj));
        }
    }
}