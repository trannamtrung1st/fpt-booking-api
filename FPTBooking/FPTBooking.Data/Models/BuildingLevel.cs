using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BuildingLevel
    {
        public BuildingLevel()
        {
            AreaLevel = new HashSet<AreaLevel>();
            Room = new HashSet<Room>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }
        public string BuildingBlockCode { get; set; }

        public virtual BuildingBlock BuildingBlockCodeNavigation { get; set; }
        public virtual ICollection<AreaLevel> AreaLevel { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
