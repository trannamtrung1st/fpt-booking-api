using System;
using System.Collections.Generic;
using System.Text;

namespace FPTBooking.FAPClient.Models
{
    public class FAPSlot
    {
        public int Slot { get; set; }
        public int CampusID { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
    }
}
