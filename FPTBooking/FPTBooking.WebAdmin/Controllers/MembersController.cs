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
using FPTBooking.Business.Services;
using FPTBooking.Data.Models;
using FirebaseAdmin.Auth;
using FPTBooking.Data.Helpers;
using FPTBooking.Business.Helpers;
using FPTBooking.Data;
using FPTBooking.Business.Queries;
using FPTBooking.WebHelpers;

namespace FPTBooking.WebAdmin.Controllers
{

    [Route(ApiEndpoint.MEMBER_API)]
    [ApiController]
    [InjectionFilter]
    public class MembersController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly MemberService _service;
        [Inject]
        private readonly SystemService _sysService;

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]MemberQueryFilter filter,
            [FromQuery]MemberQuerySort sort,
            [FromQuery] Business.Models.MemberQueryProjection projection,
            [FromQuery]MemberQueryPaging paging,
            [FromQuery]MemberQueryOptions options)
        {
            var validationData = _service.ValidateGetMembers(
               filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryMembersDynamic(
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

    }
}