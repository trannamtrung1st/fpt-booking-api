using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int ResourceCategoryId { get; set; }

        public virtual ResourceCategory ResourceCategory { get; set; }
    }
}
