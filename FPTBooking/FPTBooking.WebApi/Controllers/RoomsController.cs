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
using FPTBooking.Business.Queries;
using FPTBooking.Business.Helpers;
using FPTBooking.Data;
using FirebaseAdmin.Messaging;
using FPTBooking.Data.Models;

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.ROOM_API)]
    [ApiController]
    [InjectionFilter]
    public class RoomsController : BaseController
    {

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly RoomBusinessService _service;
        [Inject]
        private readonly MemberService _memberService;
        [Inject]
        private readonly SystemService _sysService;

#if !DEBUG
        [Authorize]
#endif
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]RoomQueryFilter filter,
            [FromQuery]RoomQuerySort sort,
            [FromQuery]RoomQueryProjection projection,
            [FromQuery]RoomQueryPaging paging,
            [FromQuery]RoomQueryOptions options)
        {
            var validationData = _service.ValidateGetRooms(
                filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryRoomDynamic(UserId,
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

#if !DEBUG
        [Authorize]
#endif
        [HttpGet("{code}")]
        public IActionResult GetDetail(string code,
            [FromQuery]RoomQueryProjection projection,
            [FromQuery]RoomQueryOptions options,
            bool hanging = false)
        {
            var checkerValid = projection.GetFieldsArr().Contains(RoomQueryProjection.CHECKER_VALID);
            projection = new RoomQueryProjection { fields = RoomQueryProjection.DETAIL };
            var entity = _service.Rooms.Code(code).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateGetRoomDetail(entity, hanging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            if (hanging)
            {
                _service.ReleaseHangingRoomByHangingUserId(UserId);
                _service.ChangeRoomHangingStatus(entity, true, UserId);
                context.SaveChanges();
            }
            var obj = _service.GetRoomDynamic(entity, projection, options);
            if (checkerValid)
            {
                var isRoomChecker = User.IsInRole(RoleName.ROOM_CHECKER);
                var valid = _memberService.AreaMembers.OfArea(entity.BuildingAreaCode).Select(o => o.MemberId)
                    .Contains(UserId) && isRoomChecker;
                obj["checker_valid"] = valid;
            }
            return Ok(AppResult.Success(data: obj));
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ROOM_CHECKER)]
#endif
        [HttpPatch("{code}/status")]
        public async Task<IActionResult> CheckRoomStatus(string code,
            CheckRoomStatusModel model)
        {
            var entity = _service.Rooms.Code(code).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateCheckRoomStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var trans = context.Database.BeginTransaction())
            {
                var oldNote = entity.Note;
                var oldStatus = entity.IsAvailable;
                _service.CheckRoomStatus(model, entity);
                //log event
                var ev = _sysService.GetEventForRoomProcessing(
                    $"{UserEmail} has changed the status of room {entity.Code}",
                    "CheckRoomStatus", UserId, new
                    {
                        old_note = oldNote,
                        old_status = oldStatus,
                        new_note = entity.Note,
                        new_status = entity.IsAvailable
                    });
                _sysService.CreateAppEvent(ev);
                //end log event
                trans.Commit();
                context.SaveChanges();
            }
            //notify managers
            var managerIds = _memberService.QueryManagersOfDepartment(entity.DepartmentCode)
                .Union(_memberService.QueryManagersOfArea(entity.BuildingAreaCode))
                .Select(o => o.UserId).ToList();
            if (managerIds.Count > 0)
                await NotiHelper.Notify(managerIds, new Notification
                {
                    Title = $"Status of room {entity.Code} has been changed",
                    Body = $"{UserEmail} has changed the status of room {entity.Code}. Pressed for more detail"
                });
            return NoContent();
        }

#if !DEBUG
        [Authorize]
#endif
        [HttpPut("hanging")]
        public IActionResult ChangeRoomHangingStatus(
            ChangeRoomHangingStatusModel model)
        {
            Room entity = null;
            if (model.Code != null)
            {
                entity = _service.Rooms.Code(model.Code).FirstOrDefault();
                if (entity == null) return NotFound(AppResult.NotFound());
            }
            var validationData = _service.ValidateChargeRoomHangingStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            if (entity != null && _service.ChangeRoomHangingStatus(entity, model.Hanging, UserId))
                context.SaveChanges();
            else
            {
                _service.ReleaseHangingRoomByHangingUserId(model.ReleaseHangingUserId);
                context.SaveChanges();
            }
            return NoContent();
        }

    }
}