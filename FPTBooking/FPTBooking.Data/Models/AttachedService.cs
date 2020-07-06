using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class AttachedService
    {
        public int Id { get; set; }
        public string BookingServiceCode { get; set; }
        public int BookingId { get; set; }
        public int NumOfPeople { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string AdminMessage { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual BookingService BookingService { get; set; }
    }
}
