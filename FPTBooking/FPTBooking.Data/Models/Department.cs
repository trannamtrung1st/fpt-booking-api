using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class Department
    {
        public Department()
        {
            DepartmentMember = new HashSet<DepartmentMember>();
            Room = new HashSet<Room>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<DepartmentMember> DepartmentMember { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
