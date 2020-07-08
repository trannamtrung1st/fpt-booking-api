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
            if (Settings.Instance.Mocking.Enabled)
            {
                var rd = new Random();
                Func<string> randomCode = () =>
                    rd.RandomStringFrom(RandomExtension.Uppers_Digits, 4);
                var list = new List<object>
                {
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                };
                list.AddRange(list);
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return Ok(AppResult.Success(data: new
                        {
                            list = list
                        }));
                    case 5:
                    case 6:
                        throw new Exception("Test exception");
                }
            }
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
            if (Settings.Instance.Mocking.Enabled)
            {
                var rd = new Random();
                var randomCode = rd.RandomStringFrom(RandomExtension.Uppers_Digits, 4);
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return Ok(AppResult.Success(data: new
                        {
                            single = new
                            {
                                code = randomCode,
                                name = randomCode,
                                area_size = rd.Next(10, 30),
                                people_capacity = rd.Next(10, 30),
                                description = "This is the room description",
                                room_type = new
                                {
                                    code = "CR",
                                    name = "Classroom"
                                },
                                block = new
                                {
                                    code = "B1",
                                    name = "Block 1"
                                },
                                level = new
                                {
                                    code = "F1",
                                    name = "Floor 1"
                                },
                                is_available = true,
                                area = new
                                {
                                    code = "CR",
                                    name = "Classroom"
                                },
                                department = new
                                {
                                    code = "D1",
                                    name = "Department of Education"
                                },
                                resources = new List<object>
                                {
                                    new
                                    {
                                        id = 1,
                                        code = "PRJ",
                                        name = "Projector",
                                        is_available = true,
                                    },
                                    new
                                    {
                                        id = 2,
                                        code = "SCR",
                                        name = "Display screen",
                                        is_available = false,
                                    }
                                },
                                note = "This is note from Room checker"
                            }
                        }));
                    case 5:
                    case 6:
                        throw new Exception("Test exception");
                }
            }
            var checkerValid = projection.GetFieldsArr().Contains(RoomQueryProjection.CHECKER_VALID);
            projection = new RoomQueryProjection { fields = RoomQueryProjection.DETAIL };
            var entity = _service.GetRoomDetail(code, projection);
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateGetRoomDetail(entity, hanging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            if (hanging)
            {
                entity = _service.Attach(entity);
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
            if (Settings.Instance.Mocking.Enabled)
                return NoContent();
            var entity = _service.Rooms.Code(code).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateCheckRoomStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.CheckRoomStatus(model, entity);
            context.SaveChanges();
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
        [HttpPut("{code}/hanging")]
        public IActionResult ChangeRoomHangingStatus(string code,
            ChangeRoomHangingStatusModel model)
        {
            if (Settings.Instance.Mocking.Enabled)
                return NoContent();
            var entity = _service.Rooms.Code(code).FirstOrDefault();
            if (entity == null) return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateHangRoom(entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            if (_service.ChangeRoomHangingStatus(entity, model.Hanging, UserId))
                context.SaveChanges();
            return NoContent();
        }

    }
}