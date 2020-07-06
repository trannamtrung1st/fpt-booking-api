﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using FPTBooking.Data.Models;

namespace FPTBooking.Data.Helpers
{

    public static class LoggingHelper
    {

        public static Logger CustomProperties(this Logger logger,
            AppUser user, object data = null)
        {
            return logger.WithProperty("Data", data != null ? JsonConvert.SerializeObject(data) : null)
                .WithProperty("UserId", user?.Id)
                .WithProperty("UserName", user?.UserName);
        }

        public static Logger CustomProperties(this Logger logger,
            ClaimsPrincipal user, object data = null)
        {
            return logger.WithProperty("Data", data != null ? JsonConvert.SerializeObject(data) : null)
                .WithProperty("UserId", user?.Identity.Name);
        }
    }
}
