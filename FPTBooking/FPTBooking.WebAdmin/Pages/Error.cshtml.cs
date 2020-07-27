using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FPTBooking.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FPTBooking.WebAdmin.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [InjectionFilter]
    public class ErrorModel : SingleHandlePageModel<ErrorModel>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }

        protected override IActionResult OnAllMethod()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            if (exception == null) return LocalRedirect(Routing.DASHBOARD);
            SetPageInfo();
#if !RELEASE
            Message = exception.Message;
#else
            Message = L["mess.main"].Value;
#endif
            _logger.Error(exception);
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return Page();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Error",
                BackUrl = BackUrl ?? Routing.DASHBOARD
            };
            Message = "Nothing";
        }
    }
}
