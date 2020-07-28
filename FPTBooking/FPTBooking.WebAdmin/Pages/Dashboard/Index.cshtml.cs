using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FPTBooking.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FPTBooking.WebAdmin.Pages.Dashboard
{
    [InjectionFilter]
    public class IndexModel : BasePageModel<IndexModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Menu = Menu.DASHBOARD,
                Title = "Dashboard",
                BackUrl = BackUrl ?? Routing.DASHBOARD
            };
        }
    }
}
