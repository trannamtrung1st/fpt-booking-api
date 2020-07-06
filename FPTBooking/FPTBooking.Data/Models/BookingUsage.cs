using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BookingUsage
    {
        public BookingUsage()
        {
            UsageOfBooking = new HashSet<UsageOfBooking>();
        }

        public int Id { get; set; }
        public int Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<UsageOfBooking> UsageOfBooking { get; set; }
    }
}
