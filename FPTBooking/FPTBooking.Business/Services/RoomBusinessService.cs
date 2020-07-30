using FPTBooking.Business.Clients;
using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class RoomBusinessService : Service
    {
        public RoomBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        [Inject]
        protected readonly MemberService _memberService;
        [Inject]
        protected readonly FptFapClient _fapClient;

        #region Create Room
        public async Task<int> SyncRoomWithFapAsync()
        {
            var rooms = await _fapClient.GetAllRooms();
            var area3Rooms = rooms.Where(o => o.AreaId == 3).ToList();
            foreach (var r in area3Rooms)
            {
                var existed = Rooms.Code(r.RoomNo).Any();
                Room entity = r.ToRoom();
                if (existed)
                    context.Room.Update(entity);
                else
                    context.Room.Add(entity);
            }
            return area3Rooms.Count;
        }
        #endregion

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

        public bool ChangeRoomHangingStatus(Room entity, bool hanging, string hangUserId)
        {
            if (hanging)
            {
                var now = DateTime.UtcNow;
                entity.HangingStartTime = now;
                entity.HangingEndTime = now.AddMinutes(10);
                entity.HangingUserId = hangUserId;
                return true;
            }
            else if (entity.HangingEndTime != null)
            {
                entity.HangingStartTime = null;
                entity.HangingEndTime = null;
                entity.HangingUserId = null;
                return true;
            }
            return false;
        }


        public IEnumerable<Room> ReleaseHangingRoomByHangingUserId(string hangingUserId)
        {
            var entities = Rooms.OfHangingUserId(hangingUserId).ToList();
            foreach (var e in entities)
                ChangeRoomHangingStatus(e, false, null);
            return entities;
        }


        public Room CheckRoomStatus(CheckRoomStatusModel model, Room entity)
        {
            model.CopyTo(entity);
            var map = entity.RoomResource.ToDictionary(o => o.Id);
            foreach (var o in model.CheckResources)
                map[o.Id].IsAvailable = o.IsAvailable;
            return entity;
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
                            obj["active_from_time"] = entity.ActiveFromTime.ToString("hh\\:mm");
                            obj["active_to_time"] = entity.ActiveToTime.ToString("hh\\:mm");
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
                                    iso = $"{time.ToUtc():s}Z"
                                };
                                time = entity.HangingEndTime.Value
                                   .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                                timeStr = time.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                                obj["hanging_end"] = new
                                {
                                    display = timeStr,
                                    iso = $"{time.ToUtc():s}Z"
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
                            if (entity != null)
                                obj["area"] = new
                                {
                                    code = entity.Code,
                                    name = entity.Name
                                };
                        }
                        break;
                    case RoomQueryProjection.BLOCK:
                        {
                            var entity = row.BuildingLevel?.BuildingBlock;
                            if (entity != null)
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
                            if (entity != null)
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
                            if (entity != null)
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
                                id = o.Id,
                                name = o.Name,
                                code = o.Code,
                                is_available = o.IsAvailable,
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
            string userId,
            RoomQueryProjection projection,
            IDictionary<string, object> tempData = null,
            RoomQueryFilter filter = null,
            RoomQuerySort sort = null,
            RoomQueryPaging paging = null,
            RoomQueryOptions options = null)
        {
            var query = Rooms.AsNoTracking();
            if (filter != null)
                query = await query.FilterAsync(filter, userId, tempData, context.Booking);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
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
                    countTask = countQuery.CountAsync();
                #endregion
            }
            if (options != null && options.count_total) totalCount = await countTask;
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
                if (filter.from_time >= filter.to_time)
                    validationData.Fail(mess: "Time range is not valid", AppResultCode.FailValidation);
                if (validationData.IsValid)
                {
                    DateTime currentTime = DateTime.UtcNow;
                    var bookedTime = filter.date?.AddMinutes(filter.from_time.Value.TotalMinutes);
                    if (currentTime >= bookedTime)
                        validationData.Fail(mess: "Booked time must be greater than current", AppResultCode.FailValidation);
                }
            }
            return validationData;
        }

        public ValidationData ValidateGetRoomDetail(
            Room entity, bool hanging,
            RoomQueryOptions options)
        {
            var validationData = new ValidationData();
            var now = DateTime.UtcNow;
            if (entity.HangingEndTime > now && hanging)
                validationData.Fail(mess: "Room is being booked. Please come back later", AppResultCode.AccessDenied);
            return validationData;
        }

        public ValidationData ValidateChargeRoomHangingStatus(
            ClaimsPrincipal principal,
            Room entity, ChangeRoomHangingStatusModel model)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            var now = DateTime.UtcNow;
            if (entity != null)
            {
                if (entity.HangingEndTime > now && model.Hanging)
                    validationData.Fail(mess: "Room is already hanged", AppResultCode.FailValidation);
            }
            return validationData;
        }

        public ValidationData ValidateCheckRoomStatus(
            ClaimsPrincipal principal,
            Room entity, CheckRoomStatusModel model)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            var roomValidCheckers = _memberService.AreaMembers.OfArea(entity.BuildingAreaCode)
                .Select(o => o.MemberId).ToList();
            if (!roomValidCheckers.Contains(userId))
                validationData.Fail(code: AppResultCode.AccessDenied);
            return validationData;
        }

        public ValidationData ValidateSyncRoomWithFap(
            ClaimsPrincipal principal)
        {
            var validationData = new ValidationData();
            return validationData;
        }
        #endregion
    }
}
