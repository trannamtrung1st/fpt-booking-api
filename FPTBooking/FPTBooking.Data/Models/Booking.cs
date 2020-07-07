using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class Booking
    {
        public Booking()
        {
            AttachedService = new HashSet<AttachedService>();
            BookingHistory = new HashSet<BookingHistory>();
            UsageOfBooking = new HashSet<UsageOfBooking>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime BookedDate { get; set; }
        public int NumOfPeople { get; set; }
        public string Note { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string RoomCode { get; set; }
        public string Status { get; set; }
        public bool Archived { get; set; }
        public string BookMemberId { get; set; }
        public string UsingMemberIds { get; set; }
        public string ManagerMessage { get; set; }
        public string Feedback { get; set; }

        public virtual Member BookMember { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<AttachedService> AttachedService { get; set; }
        public virtual ICollection<BookingHistory> BookingHistory { get; set; }
        public virtual ICollection<UsageOfBooking> UsageOfBooking { get; set; }
    }
}
