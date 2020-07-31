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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FPTBooking.Business.Clients;

namespace FPTBooking.WebAdmin.Controllers
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
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpDelete("{code}")]
        public IActionResult Delete(string code)
        {
            try
            {
                var entity = _service.Rooms.Code(code).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteRoom(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                using (var trans = context.Database.BeginTransaction())
                {
                    _service.DeleteRoom(entity);
                    //log event
                    var ev = _sysService.GetEventForDeleteRoom(
                        $"Admin {UserEmail} deleted room {entity.Code}", User,
                        entity);
                    _sysService.CreateAppEvent(ev);
                    //end log event
                    context.SaveChanges();
                    trans.Commit();
                }
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(AppResult.FailValidation(data: new ValidationData()
                    .Fail(code: AppResultCode.DependencyDeleteFail)));
            }
        }


#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPatch("{code}")]
        public IActionResult Update(string code, UpdateRoomModel model)
        {
            var entity = _service.Rooms.Code(code).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateRoom(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var transaction = context.Database.BeginTransaction())
            {
                _service.UpdateRoom(entity, model);
                //log event
                var ev = _sysService.GetEventForUpdateRoom(
                    $"Admin {UserEmail} updated room {entity.Name}",
                    User, model);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                transaction.Commit();
            }
            return NoContent();
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPost("fap-sync")]
        public async Task<IActionResult> SyncRoomWithFAP()
        {
            var validationData = _service.ValidateSyncRoomWithFap(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            int updated;
            using (var trans = context.Database.BeginTransaction())
            {
                var fapClient = HttpContext.RequestServices.GetRequiredService<FptFapClient>();
                updated = await _service.SyncRoomWithFapAsync(fapClient);
                //log event
                var ev = _sysService.GetEventForSyncRoomWithFap(
                    $"Admin {UserEmail} synced rooms information with FAP", User);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            return Ok(AppResult.Success(data: updated));
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPost("")]
        public IActionResult CreateRoom(CreateRoomModel model)
        {
            var validationData = _service.ValidateCreateRoom(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var trans = context.Database.BeginTransaction())
            {
                var entity = _service.CreateRoom(model);
                //log event
                var ev = _sysService.GetEventForCreateRoom(
                    $"Admin {UserEmail} created a new room", User, entity);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
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

    }
}