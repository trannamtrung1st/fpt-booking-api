using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class DepartmentMember
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public string DepartmentCode { get; set; }
        public bool? IsManager { get; set; }

        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual Member Member { get; set; }
    }
}
