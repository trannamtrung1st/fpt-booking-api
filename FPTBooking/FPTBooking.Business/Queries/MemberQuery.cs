using FPTBooking.Business.Models;
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
        public static IQueryable<Member> IsViewOnlyUser(this IQueryable<Member> query)
        {
            return query.Where(o => !o.DepartmentMember.Any() && !o.AreaMember.Any());
        }

        public static IQueryable<Member> IsManagerOfAny(this IQueryable<Member> query, IEnumerable<string> depCodes)
        {
            return query.Where(o => o.DepartmentMember
                .Any(dm => dm.IsManager == true && depCodes.Contains(dm.DepartmentCode)));
        }

        public static bool IsManagerOfAny(this Member entity, IEnumerable<string> depCodes)
        {
            return entity.DepartmentMember
                .Any(dm => dm.IsManager == true && depCodes.Contains(dm.DepartmentCode));
        }

        public static IQueryable<Member> IsManagerOf(this IQueryable<Member> query, string areaCode)
        {
            return query.Where(o => o.AreaMember.Any(am => am.AreaCode == areaCode));
        }

        public static IQueryable<Member> ByEmail(this IQueryable<Member> query, string email)
        {
            return query.Where(o => o.Email == email);
        }

        public static IQueryable<Member> BySearch(this IQueryable<Member> query, string search)
        {
            return query.Where(o => o.Email.Contains(search) || o.FullName.Contains(search));
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

        #region Query
        public static IQueryable<Member> Filter(this IQueryable<Member> query, MemberQueryFilter model,
            IDictionary<string, object> tempData)
        {
            if (model.search != null)
                query = query.BySearch(model.search);
            return query;
        }

        public static IQueryable<Member> Sort(this IQueryable<Member> query, MemberQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case MemberQuerySort.NAME:
                        {
                            if (asc) query = query.OrderBy(o => o.FullName);
                            else query = query.OrderByDescending(o => o.FullName);
                        }
                        break;
                    case MemberQuerySort.EMAIL:
                        {
                            if (asc) query = query.OrderBy(o => o.Email);
                            else query = query.OrderByDescending(o => o.Email);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Member> Project(this IQueryable<Member> query, Models.MemberQueryProjection model)
        {
            bool dep = false;
            foreach (var f in model.GetFieldsArr())
            {
                switch (f)
                {
                    case Models.MemberQueryProjection.DEPARTMENT: dep = true; break;
                }
            }
            query = query.Select(o => new Member
            {
                AppEvent = null,
                AreaMember = null,
                Booking = null,
                BookingHistory = null,
                Code = o.Code,
                DepartmentMember = dep ? o.DepartmentMember.Select(dm => new DepartmentMember
                {
                    DepartmentCode = dm.DepartmentCode,
                    Department = new Department
                    {
                        Name = dm.Department.Name,
                        Code = dm.DepartmentCode
                    },
                    Id = dm.Id,
                    IsManager = dm.IsManager,
                }).ToList() : null,
                Email = o.Email,
                FirstName = o.FirstName,
                FullName = o.FullName,
                LastName = o.LastName,
                MiddleName = o.MiddleName,
                Phone = o.Phone,
                UserId = o.UserId
            });
            return query;
        }
        #endregion

    }
}
