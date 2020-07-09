﻿using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Helpers
{
    public static class DateTimeHelper
    {
        public static bool TryConvertToDateTime(this string str, string dateFormat, out DateTime dateTime)
        {
            if (dateFormat == null) dateFormat = AppDateTimeFormat.DEFAULT_DATE_FORMAT;
            return DateTime.TryParseExact(s: str,
                    format: dateFormat,
                    provider: CultureInfo.InvariantCulture, style: DateTimeStyles.None, out dateTime);
        }

        public static bool TryConvertToTimeSpan(this string str, out TimeSpan timeSpan)
        {
            return TimeSpan.TryParse(str, out timeSpan);
        }

        public static string ToString(this DateTime dateTime, string dateFormat = null,
            string culture = null, string lang = null)
        {
            string timeStr;
            if (dateFormat != null) timeStr = dateTime.ToString(dateFormat);
            else if (culture != null) timeStr = dateTime.ToString(new CultureInfo(culture));
            else if (lang != null) timeStr = dateTime.ToString(new CultureInfo(lang));
            else timeStr = dateTime.ToString();
            return timeStr;
        }

        public static DateTime ToTimeZone(this DateTime dateTime, string timeZoneId = null,
            string culture = null, string lang = null)
        {
            if (timeZoneId != null) dateTime = dateTime.ToTimeZone(timeZoneId);
            else if (culture != null) dateTime = dateTime.ToTimeZone(AppTimeZone.Map[culture]);
            else if (lang != null) dateTime = dateTime.ToTimeZone(AppTimeZone.Map[lang]);
            else dateTime = dateTime.ToTimeZone(AppTimeZone.Map.Values.First());
            return dateTime;
        }

        public static DateTime ToTimeZone(this DateTime utcDate, string timeZoneId)
        {
            TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, sourceTimeZone);
            return localDate;
        }

        public static DateTime ToTimeZone(this DateTime utcDate, TimeZoneInfo timeZoneInfo)
        {
            DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZoneInfo);
            return localDate;
        }

        public static DateTime ToUtc(this DateTime localDate, TimeZoneInfo srcTimeZoneInfo = null)
        {
            srcTimeZoneInfo = srcTimeZoneInfo ?? AppTimeZone.Map.First().Value;
            return TimeZoneInfo.ConvertTimeToUtc(localDate, srcTimeZoneInfo);
        }

        public static DateTime ToDefaultTimeZone(this DateTime dateTime)
        {
            return dateTime.ToTimeZone(AppTimeZone.Map.First().Value);
        }

    }
}
