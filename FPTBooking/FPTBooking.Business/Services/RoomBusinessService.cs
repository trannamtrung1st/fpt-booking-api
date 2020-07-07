﻿using FPTBooking.Business.Helpers;
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
    public class RoomBusinessService : Service
    {
        public RoomBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Room
        public IQueryable<Room> Rooms
        {
            get
            {
                return context.Room;
            }
        }

        public IDictionary<string, object> GetRoomDynamic(
            Room row, RoomQueryProjection projection,
            RoomQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case RoomQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["archived"] = entity.Archived;
                            obj["area_size"] = entity.AreaSize;
                            obj["is_available"] = entity.IsAvailable;
                            obj["area_code"] = entity.BuildingAreaCode;
                            obj["block_code"] = entity.BuildingBlockCode;
                            obj["level_code"] = entity.BuildingLevelCode;
                            obj["code"] = entity.Code;
                            obj["department_code"] = entity.DepartmentCode;
                            obj["description"] = entity.Description;
                            //obj[""] = entity.HangingStartTime;
                            //obj[""] = entity.HangingEndTime;
                            obj["name"] = entity.Name;
                            obj["people_capacity"] = entity.PeopleCapacity;
                            obj["room_type_code"] = entity.RoomTypeCode;
                            obj["status"] = entity.Status;
                        }
                        break;
                    case RoomQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomQueryProjection.AREA:
                        {
                            var entity = row.BuildingArea;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomQueryProjection.BLOCK:
                        {
                            var entity = row.BuildingLevel.BuildingBlock;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomQueryProjection.DEPARTMENT:
                        {
                            var entity = row.Department;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomQueryProjection.LEVEL:
                        {
                            var entity = row.BuildingLevel;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case RoomQueryProjection.ROOM_TYPE:
                        {
                            var entity = row.RoomType;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetRoomDynamic(
            IEnumerable<Room> rows, RoomQueryProjection projection,
            RoomQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetRoomDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryRoomDynamic(
            RoomQueryProjection projection,
            IDictionary<string, object> tempData = null,
            RoomQueryFilter filter = null,
            RoomQuerySort sort = null,
            RoomQueryPaging paging = null,
            RoomQueryOptions options = null)
        {
            var query = Rooms;
            if (filter != null)
                query = query.Filter(filter, tempData, context.Booking);
            int? totalCount = null; Task<int> countTask = null;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !RoomQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetRoomDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var results = GetRoomDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetRooms(
            RoomQueryFilter filter,
            RoomQuerySort sort,
            RoomQueryProjection projection,
            RoomQueryPaging paging,
            RoomQueryOptions options)
        {
            var validationData = new ValidationData();
            if (filter.empty)
            {
                if (filter.date_str == null ||
                    filter.from_time == null ||
                    filter.to_time == null ||
                    filter.num_of_people == null || filter.room_type == null)
                    validationData.Fail(mess: "Invalid input data", AppResultCode.FailValidation);
                if (filter.date_str != null)
                {
                    DateTime dateTime;
                    if (filter.date_str.TryConvertToUTC(dateFormat: options.date_format, out dateTime))
                        validationData.Fail(mess: "Invalid date time format", AppResultCode.FailValidation);
                    else validationData.TempData["date"] = dateTime;
                }
            }
            return validationData;
        }

        #endregion
    }
}
