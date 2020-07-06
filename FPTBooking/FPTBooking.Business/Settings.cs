﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FPTBooking.Business
{
    public class Settings
    {
        private string[] _supportedLangs;
        public string[] SupportedLangs
        {
            get
            {
                return _supportedLangs;
            }
            set
            {
                _supportedLangs = value;
                if (value != null)
                    SupportedCultures = value.Select(c => new CultureInfo(c)).ToArray();
            }
        }
        public CultureInfo[] SupportedCultures { get; set; }
        public string Name { get; set; }

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
