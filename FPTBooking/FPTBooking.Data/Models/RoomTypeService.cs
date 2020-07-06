using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class RoomTypeService
    {
        public int Id { get; set; }
        public string BookingServiceCode { get; set; }
        public string RoomTypeCode { get; set; }

        public virtual BookingService BookingServiceCodeNavigation { get; set; }
        public virtual RoomType RoomTypeCodeNavigation { get; set; }
    }
}
