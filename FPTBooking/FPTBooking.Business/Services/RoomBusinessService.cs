using FPTBooking.Business.Helpers;
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

        public Room GetRoomDetail(string code, RoomQueryProjection projection)
        {
            var entity = Rooms.Code(code).Project(projection).FirstOrDefault();
            return entity;
        }

        public Room Attach(Room entity)
        {
            return context.Attach(entity).Entity;
        }

        public bool ChangeRoomHangingStatus(Room entity, bool hanging)
        {
            if (hanging)
            {
                var now = DateTime.UtcNow;
                entity.HangingStartTime = now;
                entity.HangingEndTime = now.AddMinutes(10);
                return true;
            }
            else if (entity.HangingEndTime != null)
            {
                entity.HangingStartTime = null;
                entity.HangingEndTime = null;
                return true;
            }
            return false;
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
                            if (entity.HangingEndTime > DateTime.UtcNow)
                            {
                                var time = entity.HangingStartTime.Value
                                   .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                                var timeStr = time.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                                obj["hanging_start"] = new
                                {
                                    display = timeStr,
                                    iso = $"{time.ToUniversalTime():s}Z"
                                };
                                time = entity.HangingEndTime.Value
                                   .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                                timeStr = time.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                                obj["hanging_end"] = new
                                {
                                    display = timeStr,
                                    iso = $"{time.ToUniversalTime():s}Z"
                                };
                            }
                            obj["name"] = entity.Name;
                            obj["people_capacity"] = entity.PeopleCapacity;
                            obj["room_type_code"] = entity.RoomTypeCode;
                            obj["note"] = entity.Note;
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
                            obj["area"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case RoomQueryProjection.BLOCK:
                        {
                            var entity = row.BuildingLevel.BuildingBlock;
                            obj["block"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case RoomQueryProjection.DEPARTMENT:
                        {
                            var entity = row.Department;
                            obj["department"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case RoomQueryProjection.LEVEL:
                        {
                            var entity = row.BuildingLevel;
                            obj["level"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case RoomQueryProjection.ROOM_TYPE:
                        {
                            var entity = row.RoomType;
                            obj["room_type"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case RoomQueryProjection.RESOURCES:
                        {
                            var entities = row.RoomResource.Select(o => new
                            {
                                name = o.Name,
                                code = o.Code,
                            }).ToList();
                            obj["resources"] = entities;
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
                if (filter.date == null ||
                    filter.from_time == null ||
                    filter.to_time == null ||
                    filter.num_of_people == null || filter.room_type == null)
                    validationData.Fail(mess: "Invalid input data", AppResultCode.FailValidation);
            }
            return validationData;
        }

        public ValidationData ValidateGetRoomDetail(
            Room room, bool hanging,
            RoomQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        public ValidationData ValidateHangRoom(
            string code, ChangeRoomHangingStatusModel model)
        {
            var validationData = new ValidationData();
            return validationData;
        }
        #endregion
    }
}
