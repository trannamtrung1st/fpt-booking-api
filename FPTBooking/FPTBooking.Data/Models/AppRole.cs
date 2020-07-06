using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FPTBooking.Data.Models
{
    public class AppRole : IdentityRole<string>
    {
        public AppRole()
        {

        }
        public AppRole(string roleName) : base(roleName: roleName)
        {

        }
    }
}
