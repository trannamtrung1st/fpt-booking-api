using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BookingHistory
    {
        public string Id { get; set; }
        public string DisplayContent { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public int BookingId { get; set; }
        public string MemberId { get; set; }
        public DateTime HappenedTime { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Member Member { get; set; }
    }
}
