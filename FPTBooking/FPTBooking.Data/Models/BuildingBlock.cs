using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class BuildingBlock
    {
        public BuildingBlock()
        {
            BuildingLevel = new HashSet<BuildingLevel>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<BuildingLevel> BuildingLevel { get; set; }
    }
}
