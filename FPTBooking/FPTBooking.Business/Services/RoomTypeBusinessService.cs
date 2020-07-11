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
    public class RoomTypeBusinessService : Service
    {
        public RoomTypeBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query RoomType
        public IQueryable<RoomType> RoomTypes
        {
            get
            {
                return context.RoomType;
            }
        }

        public IDictionary<string, object> GetRoomTypeDynamic(
            RoomType row, RoomTypeQueryProjection projection,
            RoomTypeQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case RoomTypeQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["archived"] = entity.Archived;
                            obj["code"] = entity.Code;
                            obj["description"] = entity.Description;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomTypeQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomTypeQueryProjection.SERVICES:
                        {
                            var entities = row.RoomTypeService.Select(o => o.BookingService)
                                .Select(o => new
                                {
                                    name = o.Name,
                                    code = o.Code,
                                }).ToList();
                            obj["services"] = entities;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetRoomTypeDynamic(
            IEnumerable<RoomType> rows, RoomTypeQueryProjection projection,
            RoomTypeQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetRoomTypeDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryRoomTypeDynamic(
            RoomTypeQueryProjection projection,
            IDictionary<string, object> tempData = null,
            RoomTypeQueryFilter filter = null,
            RoomTypeQuerySort sort = null,
            RoomTypeQueryPaging paging = null,
            RoomTypeQueryOptions options = null)
        {
            var query = RoomTypes.AsNoTracking();
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !RoomTypeQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetRoomTypeDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetRoomTypeDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetRoomTypes(
            RoomTypeQueryFilter filter,
            RoomTypeQuerySort sort,
            RoomTypeQueryProjection projection,
            RoomTypeQueryPaging paging,
            RoomTypeQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        #endregion
    }
}
