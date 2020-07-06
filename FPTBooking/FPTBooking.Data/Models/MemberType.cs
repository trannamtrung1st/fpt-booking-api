using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class MemberType
    {
        public MemberType()
        {
            Member = new HashSet<Member>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<Member> Member { get; set; }
    }
}
