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

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.ROOM_TYPE_API)]
    [ApiController]
    [InjectionFilter]
    public class RoomTypesController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet("")]
        public IActionResult Get()
        {
            if (Settings.Instance.Mocking.Enabled || true)
            {
                var rd = new Random();
                Func<string> randomCode = () =>
                    rd.RandomStringFrom(RandomExtension.Uppers_Digits, 4);
                var list = new List<object>
                {
                    new
                    {
                        code = "CR",
                        name = "Classroom",
                    },
                    new
                    {
                        code = "LB",
                        name = "Library",
                    },
                    new
                    {
                        code = "ST",
                        name = "Studio",
                    },
                };
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        return Ok(AppResult.Success(data: new
                        {
                            list = list
                        }));
                    case 7:
                        throw new Exception("Test exception");
                }
            }
            throw new NotImplementedException();
        }

    }
}