using FPTBooking.Business.Helpers;
using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FPTBooking.FAPClient.Models
{
    public class FAPActivity
    {
        public string SubjectCode { get; set; }
        public string GroupName { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter), "M/d/yyyy h:m:s tt")]
        public DateTime Date { get; set; }
        public string Slot { get; set; }
        public string RoomNo { get; set; }
        public string SessionNo { get; set; }
        public string Lecturer { get; set; }

        public Booking ToBooking(IDictionary<string, ValueTuple<TimeSpan, TimeSpan>> slotMap)
        {
            return new Booking
            {
                Archived = false,
                BookedDate = Date,
                DepartmentAccepted = true,
                FromTime = slotMap[Slot].Item1,
                ToTime = slotMap[Slot].Item2,
                RoomCode = RoomNo,
                Code = SubjectCode
            };
        }
    }
}
