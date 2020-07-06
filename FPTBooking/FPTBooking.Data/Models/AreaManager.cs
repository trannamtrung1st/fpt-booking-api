using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class AreaManager
    {
        public int Id { get; set; }
        public string AreaCode { get; set; }
        public string MemberId { get; set; }

        public virtual BuildingArea Area { get; set; }
        public virtual Member Member { get; set; }
    }
}
