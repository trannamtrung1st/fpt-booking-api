using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.WebApi
{
    public class Mocking
    {
        public bool Enabled { get; set; }
        public string LoginRole { get; set; }
    }

    public class Settings
    {
        public Mocking Mocking { get; set; }
        public string WebRootPath { get; set; }
        public double TokenValidHours { get; set; }
        public double RefreshTokenValidHours { get; set; }
        public string FirebaseCredentials { get; set; }
        public string FapSecretFile { get; set; }

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