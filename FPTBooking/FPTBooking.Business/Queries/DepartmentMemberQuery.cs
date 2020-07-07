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

        public static IQueryable<DepartmentMember> IsManager(this IQueryable<DepartmentMember> query)
        {
            return query.Where(o => o.IsManager == true);
        }

        public static IQueryable<DepartmentMember> OfMember(this IQueryable<DepartmentMember> query, string id)
        {
            return query.Where(o => o.MemberId == id);
        }
    }
}
