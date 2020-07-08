using FPTBooking.Business;
using FPTBooking.Business.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Helpers
{
    public class DefaultDateTimeConverter : IsoDateTimeConverter
    {
        public DefaultDateTimeConverter()
        {
            this.DateTimeFormat = AppDateTimeFormat.DEFAULT_DATE_FORMAT;
            this.Culture = CultureInfo.InvariantCulture;
            this.DateTimeStyles = DateTimeStyles.None;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var dateTime = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;
                if (dateTime != null)
                    dateTime = dateTime?.ToUniversalTime();
                return dateTime;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
