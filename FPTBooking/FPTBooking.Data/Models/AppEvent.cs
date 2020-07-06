using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class AppEvent
    {
        public string Id { get; set; }
        public string DisplayContent { get; set; }
        public string UserId { get; set; }
        public DateTime HappenedTime { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }

        public virtual Member User { get; set; }
    }
}
