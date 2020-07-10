using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FPTBooking.Data.Models
{
    public class AppUser : IdentityUser<string>
    {
        public const string TBL_NAME = "AspNetUsers";
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public string MemberCode { get; set; }

        public virtual Member Member { get; set; }
    }
}
