using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class BookingServiceQuery
    {
        public static IQueryable<BookingService> Code(this IQueryable<BookingService> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<BookingService> CodeOnly(this IQueryable<BookingService> query)
        {
            return query.Select(o => new BookingService { Code = o.Code });
        }

        public static bool Exists(this IQueryable<BookingService> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<BookingService> Codes(this IQueryable<BookingService> query, IEnumerable<string> codes)
        {
            return query.Where(q => codes.Contains(q.Code));
        }

        public static IQueryable<BookingService> NameContains(this IQueryable<BookingService> query, string nameContains)
        {
            return query.Where(o => o.Name.Contains(nameContains));
        }

        #region Query
        public static IQueryable<BookingService> Filter(this IQueryable<BookingService> query, BookingServiceQueryFilter model,
            IDictionary<string, object> tempData)
        {
            if (model.code != null)
                query = query.Code(model.code);
            if (model.name_contains != null)
                query = query.NameContains(model.name_contains);
            return query;
        }

        public static IQueryable<BookingService> Sort(this IQueryable<BookingService> query, BookingServiceQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case BookingServiceQuerySort.NAME:
                        {
                            if (asc) query = query.OrderBy(o => o.Name);
                            else query = query.OrderByDescending(o => o.Name);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<BookingService> Project(this IQueryable<BookingService> query, BookingServiceQueryProjection model)
        {
            query = query.Select(o => new BookingService
            {
                Archived = o.Archived,
                Code = o.Code,
                Description = o.Description,
                Name = o.Name
            });
            return query;
        }
        #endregion

    }
}
