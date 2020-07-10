using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class Member
    {
        public Member()
        {
            AppEvent = new HashSet<AppEvent>();
            AreaMember = new HashSet<AreaMember>();
            Booking = new HashSet<Booking>();
            BookingHistory = new HashSet<BookingHistory>();
            DepartmentMember = new HashSet<DepartmentMember>();
        }

        public string UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string MiddleName { get; set; }
        public string MemberTypeCode { get; set; }

        public virtual MemberType MemberType { get; set; }
        public virtual AppUser User { get; set; }
        public virtual ICollection<AppEvent> AppEvent { get; set; }
        public virtual ICollection<AreaMember> AreaMember { get; set; }
        public virtual ICollection<Booking> Booking { get; set; }
        public virtual ICollection<BookingHistory> BookingHistory { get; set; }
        public virtual ICollection<DepartmentMember> DepartmentMember { get; set; }
    }
}
