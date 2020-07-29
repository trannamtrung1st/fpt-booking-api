using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class AreaQuery
    {

        public static IQueryable<BuildingArea> OfRoom(this IQueryable<BuildingArea> query, string roomCode)
        {
            return query.Where(o => o.Room.Any(r => r.Code == roomCode));
        }

        public static IQueryable<BuildingArea> Code(this IQueryable<BuildingArea> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<BuildingArea> CodeOnly(this IQueryable<BuildingArea> query)
        {
            return query.Select(o => new BuildingArea { Code = o.Code });
        }

        public static bool Exists(this IQueryable<BuildingArea> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<BuildingArea> Codes(this IQueryable<BuildingArea> query, IEnumerable<string> codes)
        {
            return query.Where(q => codes.Contains(q.Code));
        }

        public static IQueryable<BuildingArea> NameContains(this IQueryable<BuildingArea> query, string nameContains)
        {
            return query.Where(o => o.Name.Contains(nameContains));
        }

        public static IQueryable<BuildingArea> Archived(this IQueryable<BuildingArea> query, bool val)
        {
            return query.Where(o => o.Archived == val);
        }


        #region Query
        public static IQueryable<BuildingArea> Filter(this IQueryable<BuildingArea> query, AreaQueryFilter model,
            IDictionary<string, object> tempData)
        {
            return query;
        }

        public static IQueryable<BuildingArea> Sort(this IQueryable<BuildingArea> query, AreaQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case AreaQuerySort.NAME:
                        {
                            if (asc) query = query.OrderBy(o => o.Name);
                            else query = query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<BuildingArea> Project(this IQueryable<BuildingArea> query, Models.AreaQueryProjection model)
        {
            return query;
        }
        #endregion

    }
}
