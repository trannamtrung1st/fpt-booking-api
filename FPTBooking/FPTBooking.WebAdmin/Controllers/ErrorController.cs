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
using FPTBooking.WebAdmin.Models;
using System.Diagnostics;
using FPTBooking.WebAdmin.Helpers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FPTBooking.WebAdmin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route(Routing.ERROR_CONTROLLER)]
    [ApiController]
    [InjectionFilter]
    public class ErrorController : BaseController, IErrorModel
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Route("")]
        public IActionResult HandleException()
        {
            dynamic context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context.Error == null) return BadRequest();
            var exception = context.Error as Exception;
            _logger.Error(exception);

            var isApiRequest = context.Path.StartsWith("/api");
            if (isApiRequest)
            {
#if DEBUG
                return Error(AppResult.Error(data: exception));
#else
                return Error(AppResult.Error());
#endif
            }
            if (exception == null) return LocalRedirect(Routing.DASHBOARD);
            HttpContext.Items["model"] = this;
            SetPageInfo();
#if !RELEASE
            Message = exception.Message;
#else
            Message = "Something's wrong. Contact admin for more information";
#endif
            _logger.Error(exception);
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return this.ErrorView();

        }

        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }
        public string Layout { get; set; } = null;
        public PageInfo Info { get; set; }

        protected void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Error",
                BackUrl = Routing.DASHBOARD
            };
            Message = "Nothing";
        }
    }
}