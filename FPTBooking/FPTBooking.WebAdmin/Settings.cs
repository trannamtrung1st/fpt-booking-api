using FPTBooking.Business.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.WebAdmin
{
    public class Settings
    {
        public Mocking Mocking { get; set; }
        public double CookiePersistentHours { get; set; }
        public double TokenValidHours { get; set; }
        public double RefreshTokenValidHours { get; set; }
        public string ApiUrl { get; set; }
        public string FapSecretFile { get; set; }
        public string FirebaseCredentials { get; set; }

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }
    }

}
