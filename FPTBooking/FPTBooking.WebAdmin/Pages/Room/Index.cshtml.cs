using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPTBooking.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNT.Core.Http.DI;

namespace FPTBooking.WebAdmin.Pages.Room
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
                Title = "Rooms",
                Menu = Menu.ROOM,
                BackUrl = BackUrl ?? Routing.DASHBOARD
            };
        }
    }
}
