using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class UsageOfBooking
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int BookingUsageId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual BookingUsage BookingUsage { get; set; }
    }
}
