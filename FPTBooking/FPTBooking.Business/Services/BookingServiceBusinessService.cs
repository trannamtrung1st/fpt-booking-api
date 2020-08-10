using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class BookingServiceBusinessService : Service
    {
        public BookingServiceBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query BookingService
        public IQueryable<BookingService> BookingServices
        {
            get
            {
                return context.BookingService;
            }
        }

        public IDictionary<string, object> GetBookingServiceDynamic(
            BookingService row, BookingServiceQueryProjection projection,
            BookingServiceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case BookingServiceQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["archived"] = entity.Archived;
                            obj["code"] = entity.Code;
                            obj["description"] = entity.Description;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case BookingServiceQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetBookingServiceDynamic(
            IEnumerable<BookingService> rows, BookingServiceQueryProjection projection,
            BookingServiceQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBookingServiceDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryBookingServiceDynamic(
            BookingServiceQueryProjection projection,
            IDictionary<string, object> tempData = null,
            BookingServiceQueryFilter filter = null,
            BookingServiceQuerySort sort = null,
            BookingServiceQueryPaging paging = null,
            BookingServiceQueryOptions options = null)
        {
            var query = BookingServices.AsNoTracking();
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !BookingServiceQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = countQuery.CountAsync();
                #endregion
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var queryResult = await query.ToListAsync();
            if (options != null && options.single_only)
            {
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetBookingServiceDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetBookingServiceDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetBookingServices(
            BookingServiceQueryFilter filter,
            BookingServiceQuerySort sort,
            BookingServiceQueryProjection projection,
            BookingServiceQueryPaging paging,
            BookingServiceQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        #endregion
    }
}
