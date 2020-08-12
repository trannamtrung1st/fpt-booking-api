using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.General;

namespace FPTBooking.Business.Queries
{
    public static class BookingQuery
    {
        public static IQueryable<Booking> OfRooms(this IQueryable<Booking> query, IEnumerable<string> roomCodes)
        {
            return query.Where(o => roomCodes.Contains(o.RoomCode));
        }

        public static IQueryable<Booking> OfRoom(this IQueryable<Booking> query, string roomCode)
        {
            return query.Where(o => o.RoomCode == roomCode);
        }

        public static IQueryable<Booking> ManagedByDepsOrAreasExceptStatuses(this IQueryable<Booking> query,
            IEnumerable<string> depCodes, IEnumerable<string> depStatuses,
            IEnumerable<string> areaCodes, IEnumerable<string> areaStatuses)
        {
            return query.Where(o => (o.BookMember.DepartmentMember
                        .Select(dm => dm.DepartmentCode).Any(dmCode => depCodes.Contains(dmCode))
                    && (depStatuses == null || !depStatuses.Contains(o.Status)))
                || (areaCodes.Contains(o.Room.BuildingAreaCode) &&
                    (areaStatuses == null || !areaStatuses.Contains(o.Status)) && o.DepartmentAccepted));
        }

        public static IQueryable<Booking> OfDeps(this IQueryable<Booking> query, IEnumerable<string> depCodes)
        {
            return query.Where(o => depCodes.Contains(o.Room.DepartmentCode));
        }

        public static IQueryable<Booking> OfAreas(this IQueryable<Booking> query, IEnumerable<string> areaCodes)
        {
            return query.Where(o => areaCodes.Contains(o.Room.BuildingAreaCode));
        }

        public static IQueryable<Booking> DepartmentAccepted(this IQueryable<Booking> query, bool val)
        {
            return query.Where(o => o.DepartmentAccepted == val);
        }

        public static IQueryable<Booking> BookedDate(this IQueryable<Booking> query, DateTime date)
        {
            return query.Where(o => o.BookedDate.Date == date.Date);
        }

        public static IQueryable<Booking> SentDate(this IQueryable<Booking> query, DateTime date)
        {
            return query.Where(o => o.SentDate.Date == date.Date);
        }

        public static IQueryable<Booking> ActiveStatus(this IQueryable<Booking> query)
        {
            return query.NotStatus(BookingStatusValues.ABORTED)
                .NotStatus(BookingStatusValues.DENIED);
        }

        public static IQueryable<Booking> Search(this IQueryable<Booking> query, string search)
        {
            return query.Where(o => o.Code.Contains(search));
        }

        public static IQueryable<Booking> Status(this IQueryable<Booking> query, string status)
        {
            return query.Where(o => o.Status == status);
        }

        public static IQueryable<Booking> NotStatus(this IQueryable<Booking> query, string status)
        {
            return query.Where(o => o.Status != status);
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

        public static IQueryable<Booking> UsedByMember(this IQueryable<Booking> query, string userId)
        {
            return query.Where(o => o.UsingMemberIds.Contains(userId));
        }

        public static IQueryable<Booking> OfBookMember(this IQueryable<Booking> query, string userId)
        {
            return query.Where(o => o.BookMemberId == userId);
        }

        public static IQueryable<Booking> RelatedToMember(this IQueryable<Booking> query, string userId)
        {
            return query.Where(o => o.BookMemberId == userId || o.UsingMemberIds.Contains(userId));
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

        public static IQueryable<Booking> BookedDateFromDate(this IQueryable<Booking> query, DateTime utcDate)
        {
            return query.Where(o => o.BookedDate.Date >= utcDate.Date);
        }
        public static IQueryable<Booking> BookedDateToDate(this IQueryable<Booking> query, DateTime utcDate)
        {
            return query.Where(o => o.BookedDate.Date <= utcDate.Date);
        }
        public static IQueryable<Booking> SentDateFromDate(this IQueryable<Booking> query, DateTime utcDate)
        {
            return query.Where(o => o.SentDate.Date >= utcDate.Date);
        }
        public static IQueryable<Booking> SentDateToDate(this IQueryable<Booking> query, DateTime utcDate)
        {
            var nextDate = utcDate.AddDays(1);
            return query.Where(o => o.SentDate.Date < nextDate.Date);
        }

        public static IQueryable<Booking> SortOldestBookedDateFirst(this IQueryable<Booking> query)
        {
            return query.OrderBy(o => o.BookedDate).OrderBy(o => o.FromTime);
        }

        public static IQueryable<Booking> SortLatestBookedDateFirst(this IQueryable<Booking> query)
        {
            return query.OrderByDescending(o => o.BookedDate).OrderByDescending(o => o.FromTime);
        }

        #region Query
        public static IQueryable<Booking> Filter(this IQueryable<Booking> query, BookingQueryFilter model, IDictionary<string, object> tempData)
        {
            var archived = model.archived ?? BoolOptions.F;
            if (archived != BoolOptions.B)
                query = query.Archived(!(archived == BoolOptions.F));
            switch (model.date_type)
            {
                case BookingQueryFilterDateType.BOOKED_DATE:
                    if (model.from_date != null)
                        query = query.SentDateFromDate(model.from_date.Value);
                    if (model.to_date != null)
                    {
                        var toDateUtc = model.to_date.Value.ToEndOfDay().ToUtc();
                        query = query.SentDateToDate(toDateUtc);
                    }
                    if (model.date != null)
                        query = query.SentDate(model.date.Value);
                    break;
                case BookingQueryFilterDateType.SENT_DATE:
                    if (model.from_date != null)
                        query = query.BookedDateFromDate(model.from_date.Value);
                    if (model.to_date != null)
                    {
                        var toDateUtc = model.to_date.Value.ToEndOfDay().ToUtc();
                        query = query.BookedDateToDate(toDateUtc);
                    }
                    if (model.date != null)
                        query = query.BookedDate(model.date.Value);
                    break;
            }
            if (model.status != null)
                query = query.Status(model.status);
            if (model.search != null)
                query = query.Search(model.search);
            if (model.room_code != null)
                query = query.OfRoom(model.room_code);
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
                    case BookingQuerySort.BOOKED_DATE:
                        {
                            if (asc) query = query.SortOldestBookedDateFirst();
                            else query = query.SortLatestBookedDateFirst();
                        }
                        break;
                    case BookingQuerySort.SENT_DATE:
                        {
                            if (asc) query = query.OrderBy(o => o.SentDate);
                            else query = query.OrderByDescending(o => o.SentDate);
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
                        Id = s.Id,
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
