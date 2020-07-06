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

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.DEV_API)]
    [ApiController]
    [InjectionFilter]
    public class DevController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpPut("mocking")]
        public IActionResult ChangeMode(Mocking mocking)
        {
            Settings.Instance.Mocking = mocking;
            return Ok(mocking);
        }

        [HttpPut("{enabled}")]
        public IActionResult ChangeMode(bool enabled)
        {
            Settings.Instance.Mocking.Enabled = enabled;
            return Ok(enabled);
        }

        [HttpPost("noti")]
        public async Task<IActionResult> TestPushNoti(Message message)
        {
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return Ok(AppResult.Success(result));
        }
    }
}