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
using FirebaseAdmin.Messaging;
using FPTBooking.Data;
using FPTBooking.Business.Services;
using FPTBooking.Business.Queries;

namespace FPTBooking.WebAdmin.Controllers
{

    [Route(ApiEndpoint.ADMIN_API)]
    [ApiController]
    [InjectionFilter]
    public class AdminController : BaseController
    {
        [Inject]
        private readonly IdentityService _identityService;
        [Inject]
        private readonly AdminService _adminService;

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#endif
        [HttpPost("noti")]
        public async Task<IActionResult> PushNoti(PushNotiModel model)
        {
            var user = _identityService.Users.ByEmail(model.Email).FirstOrDefault();
            if (user == null)
                return NotFound(AppResult.NotFound());
            var result = await _adminService.PushNotiToUser(user, model);
            return Ok(AppResult.Success(result));
        }
    }
}