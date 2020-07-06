using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class BookingQuery
    {
        public static IQueryable<Booking> Id(this IQueryable<Booking> query, string id)
        {
            return query.Where(o => o.Id.Equals(id));
        }
        public static IQueryable<Booking> OfBookMember(this IQueryable<Booking> query, string userId)
        {
            return query.Where(o => o.BookMemberId.Equals(userId));
        }

        public static IQueryable<Booking> IdOnly(this IQueryable<Booking> query)
        {
            return query.Select(o => new Booking { Code = o.Code });
        }

        public static bool Exists(this IQueryable<Booking> query, string id)
        {
            return query.Any(o => o.Code.Equals(id));
        }

        public static IQueryable<Booking> Ids(this IQueryable<Booking> query, IEnumerable<string> ids)
        {
            return query.Where(q => ids.Contains(q.Code));
        }

        #region Query
        public static IQueryable<Booking> Filter(this IQueryable<Booking> query, BookingQueryFilter model, IDictionary<string, object> tempData)
        {
            if (model.archived != 2)
                query = query.Where(o => o.Archived == (model.archived != 0));
            if (model.id != null)
                query = query.Where(o => o.Id == model.id);
            if (model.name_contains != null)
                query = query.Where(o => o.Code.Contains(model.name_contains));
            if (model.from_date_str != null)
            {
                var fromDate = (DateTime)tempData["from_date"];
                query = query.Where(o => o.BookedDate.Date >= fromDate.Date);
            }
            if (model.to_date_str != null)
            {
                var toDate = (DateTime)tempData["to_date"];
                query = query.Where(o => o.BookedDate.Date <= toDate.Date);
            }
            if (model.date_str != null)
            {
                var date = (DateTime)tempData["date"];
                query = query.Where(o => o.BookedDate.Date == date.Date);
            }
            return query;
        }

        public static IQueryable<Booking> Sort(this IQueryable<Booking> query, BookingQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case BookingQuerySort.DATE:
                        {
                            if (asc) query = query.OrderBy(o => o.BookedDate)
                                                .OrderBy(o => o.FromTime);
                            else query = query.OrderByDescending(o => o.BookedDate)
                                                .OrderByDescending(o => o.FromTime);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Booking> Project(this IQueryable<Booking> query, BookingQueryProjection model)
        {
            var finalFields = model.GetFieldsArr()
                .Where(f => BookingQueryProjection.FIELDS_MAPPING.ContainsKey(f))
                .Select(f => BookingQueryProjection.FIELDS_MAPPING[f]);
            foreach (var f in finalFields)
                query = query.Include(f);
            return query;
        }
        #endregion

    }
}
