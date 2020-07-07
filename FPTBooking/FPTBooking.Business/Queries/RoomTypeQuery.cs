using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class RoomTypeQuery
    {
        public static IQueryable<RoomType> Code(this IQueryable<RoomType> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<RoomType> CodeOnly(this IQueryable<RoomType> query)
        {
            return query.Select(o => new RoomType { Code = o.Code });
        }

        public static bool Exists(this IQueryable<RoomType> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<RoomType> Codes(this IQueryable<RoomType> query, IEnumerable<string> codes)
        {
            return query.Where(q => codes.Contains(q.Code));
        }

        public static IQueryable<RoomType> NameContains(this IQueryable<RoomType> query, string nameContains)
        {
            return query.Where(o => o.Name.Contains(nameContains));
        }

        #region Query
        public static IQueryable<RoomType> Filter(this IQueryable<RoomType> query, RoomTypeQueryFilter model,
            IDictionary<string, object> tempData)
        {
            if (model.code != null)
                query = query.Code(model.code);
            if (model.name_contains != null)
                query = query.NameContains(model.name_contains);
            return query;
        }

        public static IQueryable<RoomType> Sort(this IQueryable<RoomType> query, RoomTypeQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case RoomTypeQuerySort.NAME:
                        {
                            if (asc) query = query.OrderBy(o => o.Name);
                            else query = query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<RoomType> Project(this IQueryable<RoomType> query, RoomTypeQueryProjection model)
        {
            bool services = false;
            foreach (var f in model.GetFieldsArr())
            {
                switch (f)
                {
                    case RoomTypeQueryProjection.SERVICES: services = true; break;
                }
            }
            query = query.Select(o => new RoomType
            {
                Archived = o.Archived,
                Code = o.Code,
                Description = o.Description,
                Name = o.Name,
                //Room = o.Room,
                RoomTypeService = services ? o.RoomTypeService
                    .Select(s => new RoomTypeService
                    {
                        BookingService = s.BookingService
                    }).Where(s => !s.BookingService.Archived).ToList()
                    : null
            });
            return query;
        }
        #endregion

    }
}
