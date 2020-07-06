using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPTBooking.Business.Models;
using FPTBooking.Data.Models;

namespace FPTBooking.Business.Queries
{
    public static class AppUserQuery
    {
        public static IQueryable<AppUser> Id(this IQueryable<AppUser> query, string id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<AppUser> IdOnly(this IQueryable<AppUser> query)
        {
            return query.Select(o => new AppUser { Id = o.Id });
        }

        public static bool Exists(this IQueryable<AppUser> query, string id)
        {
            return query.Any(o => o.Id == id);
        }

        public static IQueryable<AppUser> Ids(this IQueryable<AppUser> query, IEnumerable<string> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

    }
}
