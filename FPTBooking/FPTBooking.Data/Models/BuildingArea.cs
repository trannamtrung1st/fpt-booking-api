using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BuildingArea
    {
        public BuildingArea()
        {
            AreaManager = new HashSet<AreaManager>();
            Room = new HashSet<Room>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<AreaManager> AreaManager { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
