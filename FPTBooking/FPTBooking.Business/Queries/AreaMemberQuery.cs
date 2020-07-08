using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class AreaMemberQuery
    {
        public static IQueryable<AreaMember> IsNotManager(this IQueryable<AreaMember> query)
        {
            return query.Where(o => o.IsManager == false);
        }

        public static IQueryable<AreaMember> IsManager(this IQueryable<AreaMember> query)
        {
            return query.Where(o => o.IsManager == true);
        }

        public static IQueryable<AreaMember> OfMember(this IQueryable<AreaMember> query, string id)
        {
            return query.Where(o => o.MemberId == id);
        }

        public static IQueryable<AreaMember> OfArea(this IQueryable<AreaMember> query, string areaCode)
        {
            return query.Where(o => o.AreaCode == areaCode);
        }
    }
}
