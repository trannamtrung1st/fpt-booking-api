using System;
using System.Collections.Generic;
using System.Text;
using FPTBooking.Data.Models;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public abstract class Service
    {
        [Inject]
        protected readonly DataContext context;

        public Service(ServiceInjection inj)
        {
            inj.Inject(this);
        }

    }
}
