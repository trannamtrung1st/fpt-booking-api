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
        public static IQueryable<Booking> BookedDate(this IQueryable<Booking> query, DateTime date)
        {
            return query.Where(o => o.BookedDate.Date == date.Date);
        }

        public static IQueryable<Booking> ActiveStatus(this IQueryable<Booking> query)
        {
            return query.Where(o => o.Status != BookingStatusValues.ABORTED && o.Status != BookingStatusValues.DENIED);
        }

        public static IQueryable<Booking> OverlappedInTimeRange(this IQueryable<Booking> query,
            TimeSpan fromTime, TimeSpan toTime)
        {
            return query.Where(o => ((o.FromTime < toTime && o.ToTime >= toTime)
                        || (o.ToTime <= toTime && o.ToTime > fromTime)));
        }

        public static IQueryable<Booking> Overlapped(this IQueryable<Booking> query, DateTime date,
            TimeSpan fromTime, TimeSpan toTime)
        {
            return query.BookedDate(date).OverlappedInTimeRange(fromTime, toTime);
        }

        public static IQueryable<Booking> Id(this IQueryable<Booking> query, int id)
        {
            return query.Where(o => o.Id == id);
        }

        public static IQueryable<Booking> Code(this IQueryable<Booking> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<Booking> OfBookMember(this IQueryable<Booking> query, string userId)
        {
            return query.Where(o => o.BookMemberId.Equals(userId));
        }

        public static IQueryable<Booking> IdOnly(this IQueryable<Booking> query)
        {
            return query.Select(o => new Booking { Code = o.Code });
        }

        public static bool Exists(this IQueryable<Booking> query, int id)
        {
            return query.Any(o => o.Id == id);
        }
        public static bool Exists(this IQueryable<Booking> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<Booking> Ids(this IQueryable<Booking> query, IEnumerable<int> ids)
        {
            return query.Where(q => ids.Contains(q.Id));
        }

        public static IQueryable<Booking> Archived(this IQueryable<Booking> query, bool val)
        {
            return query.Where(o => o.Archived == val);
        }

        public static IQueryable<Booking> FromDate(this IQueryable<Booking> query, DateTime date)
        {
            return query.Where(o => o.BookedDate.Date >= date.Date);
        }
        public static IQueryable<Booking> ToDate(this IQueryable<Booking> query, DateTime date)
        {
            return query.Where(o => o.BookedDate.Date <= date.Date);
        }

        public static IQueryable<Booking> SortLatest(this IQueryable<Booking> query)
        {
            return query.OrderBy(o => o.BookedDate).OrderBy(o => o.FromTime);
        }

        public static IQueryable<Booking> SortOldest(this IQueryable<Booking> query)
        {
            return query.OrderByDescending(o => o.BookedDate).OrderByDescending(o => o.FromTime);
        }

        #region Query
        public static IQueryable<Booking> Filter(this IQueryable<Booking> query, BookingQueryFilter model, IDictionary<string, object> tempData)
        {
            var archived = model.archived ?? BoolOptions.F;
            if (archived != BoolOptions.B)
                query = query.Archived(!(archived == BoolOptions.F));
            if (model.id != null)
                query = query.Id(model.id.Value);
            if (model.code != null)
                query = query.Code(model.code);
            if (model.from_date != null)
                query = query.FromDate(model.from_date.Value);
            if (model.to_date != null)
                query = query.ToDate(model.to_date.Value);
            if (model.date != null)
                query = query.BookedDate(model.date.Value);
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
                            if (asc) query = query.SortLatest();
                            else query = query.SortOldest();
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Booking> Project(this IQueryable<Booking> query, BookingQueryProjection model)
        {
            bool room = false, member = false, services = false;
            foreach (var f in model.GetFieldsArr())
            {
                switch (f)
                {
                    case BookingQueryProjection.ROOM: room = true; break;
                    case BookingQueryProjection.MEMBER: member = true; break;
                    case BookingQueryProjection.SERVICES: services = true; break;
                }
            }
            query = query.Select(o => new Booking
            {
                Archived = o.Archived,
                BookedDate = o.BookedDate,
                //BookingHistory = o.BookingHistory,
                BookMemberId = o.BookMemberId,
                Code = o.Code,
                FromTime = o.FromTime,
                Id = o.Id,
                Note = o.Note,
                NumOfPeople = o.NumOfPeople,
                Room = room ? o.Room : null,
                Status = o.Status,
                UsingMemberIds = o.UsingMemberIds,
                RoomCode = o.RoomCode,
                SentDate = o.SentDate,
                ToTime = o.ToTime,
                Feedback = o.Feedback,
                ManagerMessage = o.ManagerMessage,
                AttachedService = services ? o.AttachedService.AsQueryable()
                    .Select(s => new AttachedService
                    {
                        BookingService = new BookingService
                        {
                            Code = s.BookingService.Code,
                            Name = s.BookingService.Name
                        }
                    }).ToList() : null,
                BookMember = member ? o.BookMember : null,
            });
            return query;
        }
        #endregion

    }
}
