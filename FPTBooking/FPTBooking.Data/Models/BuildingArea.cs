using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BuildingArea
    {
        public BuildingArea()
        {
            AreaMember = new HashSet<AreaMember>();
            Room = new HashSet<Room>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<AreaMember> AreaMember { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
