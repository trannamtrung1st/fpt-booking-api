using FPTBooking.Business.Helpers;
using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FPTBooking.Business.Clients.Models
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

        public Booking ToBooking()
        {
            var slots = Slot.Substring(1, Slot.Length - 1).Split('-')
                .Select(o => TimeSpan.ParseExact(o, "h:m", CultureInfo.InvariantCulture)).ToList();
            return new Booking
            {
                Archived = false,
                UsingMemberIds = Lecturer,
                Room = new Room
                {
                    Code = RoomNo,
                    Name = RoomNo
                },
                Status = BookingStatusValues.APPROVED,
                Code = SubjectCode,
                BookedDate = Date,
                BookMemberId = Booker,
                DepartmentAccepted = true,
                FromTime = slots[0],
                ToTime = slots[1],
                Note = Note,
                RoomCode = RoomNo,
            };
        }
    }
}
