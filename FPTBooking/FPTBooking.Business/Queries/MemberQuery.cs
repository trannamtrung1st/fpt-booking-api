using FPTBooking.Data;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class MemberQuery
    {
        public static IQueryable<Member> IsManagerOfAny(this IQueryable<Member> query, IEnumerable<string> depCodes)
        {
            return query.Where(o => o.DepartmentMember
                .Any(dm => dm.IsManager == true && depCodes.Contains(dm.DepartmentCode)));
        }

        public static IQueryable<Member> IsManagerOf(this IQueryable<Member> query, BuildingArea area)
        {
            return query.Where(o => o.AreaManager.Any(am => am.AreaCode == area.Code));
        }

        public static IQueryable<Member> ByEmail(this IQueryable<Member> query, string email)
        {
            return query.Where(o => o.Email == email);
        }

        public static IQueryable<Member> ByEmails(this IQueryable<Member> query, IEnumerable<string> emails)
        {
            return query.Where(o => emails.Contains(o.Email));
        }

        public static IQueryable<Member> Id(this IQueryable<Member> query, string id)
        {
            return query.Where(o => o.UserId == id);
        }

        public static IQueryable<Member> IdOnly(this IQueryable<Member> query)
        {
            return query.Select(o => new Member { UserId = o.UserId });
        }

        public static bool Exists(this IQueryable<Member> query, string id)
        {
            return query.Any(o => o.UserId == id);
        }

        public static IQueryable<Member> Ids(this IQueryable<Member> query, IEnumerable<string> ids)
        {
            return query.Where(q => ids.Contains(q.UserId));
        }
    }
}
