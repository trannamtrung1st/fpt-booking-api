﻿using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class RoomService
    {
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string RoomCode { get; set; }

        public virtual Room RoomCodeNavigation { get; set; }
        public virtual BookingService ServiceCodeNavigation { get; set; }
    }
}
