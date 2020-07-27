using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FPTBooking.WebAdmin.Helpers;
using FPTBooking.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FPTBooking.WebAdmin.Pages
{
    [InjectionFilter]
    public class StatusModel : BasePageModel<StatusModel>, IStatusModel
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public string Message { get; set; }
        public int Code { get; set; }
        public string Layout { get; set; } = null;
        public string MessageTitle { get; set; }
        public string StatusCodeStyle { get; set; } = "warning";
        public string OriginalUrl { get; set; }

        public IActionResult OnGet(int code)
        {
            var statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeReExecuteFeature == null) return LocalRedirect(Routing.DASHBOARD);
            SetPageInfo();
            OriginalUrl =
                statusCodeReExecuteFeature.OriginalPathBase
                + statusCodeReExecuteFeature.OriginalPath
                + statusCodeReExecuteFeature.OriginalQueryString;
            Code = code;
            return this.StatusView();
        }

        public IActionResult OnGetSimulate(HttpStatusCode code)
        {
            if (code == HttpStatusCode.InternalServerError)
            {
                throw new Exception("Simulated error");
            }
            return StatusCode((int)code);
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Something's wrong",
                BackUrl = BackUrl ?? Routing.DASHBOARD
            };
            Message = "There's something happened unexpectedly. Please try again or contact your admin";
            MessageTitle = "Oops...";
        }
    }
}
