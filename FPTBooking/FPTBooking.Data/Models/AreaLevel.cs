using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class AreaLevel
    {
        public int Id { get; set; }
        public string AreaCode { get; set; }
        public string LevelCode { get; set; }

        public virtual BuildingArea Area { get; set; }
        public virtual BuildingLevel Level { get; set; }
    }
}
