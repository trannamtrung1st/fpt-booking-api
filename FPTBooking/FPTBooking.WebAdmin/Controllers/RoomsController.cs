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