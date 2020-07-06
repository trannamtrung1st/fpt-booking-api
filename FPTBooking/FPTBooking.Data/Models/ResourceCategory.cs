using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class ResourceCategory
    {
        public ResourceCategory()
        {
            Resource = new HashSet<Resource>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Resource> Resource { get; set; }
    }
}
