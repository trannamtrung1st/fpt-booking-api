using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class DepartmentMemberQuery
    {
        public static IQueryable<DepartmentMember> IsNotManager(this IQueryable<DepartmentMember> query)
        {
            return query.Where(o => o.IsManager == false);
        }
    }
}
