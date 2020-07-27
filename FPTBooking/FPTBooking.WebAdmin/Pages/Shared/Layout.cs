﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.WebAdmin.Pages.Shared
{
    public class Layout
    {
        public Layout(ServiceInjection inj)
        {
            inj.Inject(this);
        }
    }
}
