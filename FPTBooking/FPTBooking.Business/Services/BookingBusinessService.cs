using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data;
using FPTBooking.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using TNT.Core.Helpers.General;

namespace FPTBooking.Business.Services
{
    public class BookingBusinessService : Service
    {
        [Inject]
        private readonly UserManager<AppUser> _userManager;
        public BookingBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Booking
        public IQueryable<Booking> Bookings
        {
            get
            {
                return context.Booking;
            }
        }

        public IDictionary<string, object> GetBookingDynamic(
            Booking row, BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case BookingQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            var bookedDate = entity.BookedDate
                               .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                            var timeStr = bookedDate.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                            obj["booked_date"] = new
                            {
                                display = timeStr,
                                iso = $"{bookedDate.ToUniversalTime():s}Z"
                            };
                            obj["from_time"] = entity.FromTime;
                            obj["to_time"] = entity.ToTime;
                            obj["type"] = BookingTypeValues.BOOKING;
                            obj["status"] = entity.Status;
                        }
                        break;
                    case BookingQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Code;
                            obj["code"] = entity.Code;
                        }
                        break;
                    case BookingQueryProjection.ROOM:
                        {
                            var entity = row.Room;
                            obj["room"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetBookingDynamic(
            IEnumerable<Booking> rows, BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBookingDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryBookingDynamic(
            ClaimsPrincipal principal,
            BookingPrincipalRelationship relationship,
            BookingQueryProjection projection,
            IDictionary<string, object> tempData = null,
            BookingQueryFilter filter = null,
            BookingQuerySort sort = null,
            BookingQueryPaging paging = null,
            BookingQueryOptions options = null)
        {
            var query = Bookings;
            //---- multi-tenants ------
            if (principal != null)
            {
                var memberId = principal.Identity.Name;
                switch (relationship)
                {
                    case BookingPrincipalRelationship.Owner:
                        query = query.OfBookMember(memberId);
                        break;
                }
            }
            //-------------------------
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !BookingQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = query.CountAsync();
                #endregion
            }
            var queryResult = await query.ToListAsync();
            if (options != null && options.single_only)
            {
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetBookingDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var results = GetBookingDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetBookings(
            ClaimsPrincipal principal,
            BookingQueryFilter filter,
            BookingQuerySort sort,
            BookingQueryProjection projection,
            BookingQueryPaging paging,
            BookingQueryOptions options)
        {
            var validationData = new ValidationData();
            if (filter.to_date_str != null)
            {
                DateTime dateTime;
                if (filter.to_date_str.TryConvertToUTC(dateFormat: options.date_format, out dateTime))
                    validationData.Fail(mess: "Invalid date time format", AppResultCode.FailValidation);
                else validationData.TempData["to_date"] = dateTime;
            }
            if (filter.from_date_str != null)
            {
                DateTime dateTime;
                if (filter.from_date_str.TryConvertToUTC(dateFormat: options.date_format, out dateTime))
                    validationData.Fail(mess: "Invalid date time format", AppResultCode.FailValidation);
                else validationData.TempData["from_date"] = dateTime;
            }
            if (filter.date_str != null)
            {
                DateTime dateTime;
                if (filter.date_str.TryConvertToUTC(dateFormat: options.date_format, out dateTime))
                    validationData.Fail(mess: "Invalid date time format", AppResultCode.FailValidation);
                else validationData.TempData["date"] = dateTime;
            }
            return validationData;
        }

        #endregion
    }
}
