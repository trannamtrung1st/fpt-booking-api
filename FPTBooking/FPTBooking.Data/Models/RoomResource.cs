using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class RoomResource
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string RoomCode { get; set; }

        public virtual Room Room { get; set; }
    }
}
