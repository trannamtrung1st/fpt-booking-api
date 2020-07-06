using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class RoomType
    {
        public RoomType()
        {
            Room = new HashSet<Room>();
            RoomTypeService = new HashSet<RoomTypeService>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Room> Room { get; set; }
        public virtual ICollection<RoomTypeService> RoomTypeService { get; set; }
    }
}
