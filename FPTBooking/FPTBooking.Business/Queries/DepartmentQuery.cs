using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class DepartmentQuery
    {

        public static IQueryable<Department> OfRoom(this IQueryable<Department> query, string roomCode)
        {
            return query.Where(o => o.Room.Any(r => r.Code == roomCode));
        }

        public static IQueryable<Department> Code(this IQueryable<Department> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<Department> CodeOnly(this IQueryable<Department> query)
        {
            return query.Select(o => new Department { Code = o.Code });
        }

        public static bool Exists(this IQueryable<Department> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<Department> Codes(this IQueryable<Department> query, IEnumerable<string> codes)
        {
            return query.Where(q => codes.Contains(q.Code));
        }

        public static IQueryable<Department> NameContains(this IQueryable<Department> query, string nameContains)
        {
            return query.Where(o => o.Name.Contains(nameContains));
        }

        public static IQueryable<Department> Archived(this IQueryable<Department> query, bool val)
        {
            return query.Where(o => o.Archived == val);
        }


        #region Query
        public static IQueryable<Department> Filter(this IQueryable<Department> query, DepartmentQueryFilter model,
            IDictionary<string, object> tempData)
        {
            return query;
        }

        public static IQueryable<Department> Sort(this IQueryable<Department> query, DepartmentQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case DepartmentQuerySort.NAME:
                        {
                            if (asc) query = query.OrderBy(o => o.Name);
                            else query = query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Department> Project(this IQueryable<Department> query, Models.DepartmentQueryProjection model)
        {
            return query;
        }
        #endregion

    }
}
