using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class AreaManagerQuery
    {
        public static IQueryable<AreaManager> OfMember(this IQueryable<AreaManager> query, string id)
        {
            return query.Where(o => o.MemberId == id);
        }

        public static IQueryable<AreaManager> OfArea(this IQueryable<AreaManager> query, string areaCode)
        {
            return query.Where(o => o.AreaCode == areaCode);
        }
    }
}
