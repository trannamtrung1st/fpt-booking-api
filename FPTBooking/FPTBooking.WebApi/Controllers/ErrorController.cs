using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.DI;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using FPTBooking.Business;
using FPTBooking.Business.Models;

namespace FPTBooking.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route(ApiEndpoint.ERROR)]
    [ApiController]
    [InjectionFilter]
    public class ErrorController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Route("")]
        public IActionResult HandleException()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context.Error == null) return BadRequest();
            var e = context.Error;
            _logger.Error(e);
#if DEBUG
            return Error(AppResult.Error(data: e));
#else
            return Error(AppResult.Error());
#endif
        }
    }
}