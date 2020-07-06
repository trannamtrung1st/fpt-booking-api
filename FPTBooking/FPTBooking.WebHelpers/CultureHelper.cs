using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FPTBooking.Business;

namespace FPTBooking.WebHelpers
{
    public static class CultureHelper
    {
        public static string CurrentLang
        {
            get
            {
                return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }
        }

        public static TimeZoneInfo DefaultTimeZone
        {
            get
            {
                return AppTimeZone.Map[CurrentLang];
            }
        }
    }
}
