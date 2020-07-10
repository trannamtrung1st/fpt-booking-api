using FPTBooking.Business.Helpers;
using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FPTBooking.FAPClient.Models
{
    public class FAPSchedule
    {
        public string CourseId { get; set; }
        public string Lecturer { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter), "M/d/yyyy h:m:s tt")]
        public DateTime Date { get; set; }
        public string RoomNo { get; set; }
        public string Slot { get; set; }
        public string Note { get; set; }
        public string Booker { get; set; }
        public string ClassName { get; set; }
        public string SubjectCode { get; set; }

        public Booking ToBooking(IDictionary<string, ValueTuple<TimeSpan, TimeSpan>> slotMap)
        {
            return new Booking
            {
                Archived = false,
                BookedDate = Date,
                BookMemberId = Booker,
                DepartmentAccepted = true,
                FromTime = slotMap[Slot].Item1,
                ToTime = slotMap[Slot].Item2,
                Note = Note,
                RoomCode = RoomNo,
            };
        }
    }
}
