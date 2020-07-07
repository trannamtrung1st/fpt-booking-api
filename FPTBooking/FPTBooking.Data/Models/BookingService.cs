using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BookingService
    {
        public BookingService()
        {
            AttachedService = new HashSet<AttachedService>();
            RoomTypeService = new HashSet<RoomTypeService>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<AttachedService> AttachedService { get; set; }
        public virtual ICollection<RoomTypeService> RoomTypeService { get; set; }
    }
}
