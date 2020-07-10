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

        public Booking ToBooking()
        {
            var slots = Slot.Substring(1, Slot.Length - 2).Split('-')
                .Select(o => TimeSpan.ParseExact(o, "h\\:m", CultureInfo.InvariantCulture)).ToList();
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
                BookedDate = Date,
                DepartmentAccepted = true,
                FromTime = slots[0],
                ToTime = slots[1],
                RoomCode = RoomNo,
                Code = SubjectCode
            };
        }
    }
}
