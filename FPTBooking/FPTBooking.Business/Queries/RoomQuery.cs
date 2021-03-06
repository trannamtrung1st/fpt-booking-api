﻿using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class RoomQuery
    {
        public static IQueryable<Room> Code(this IQueryable<Room> query, string code)
        {
            return query.Where(o => o.Code == code);
        }

        public static IQueryable<Room> CodeOnly(this IQueryable<Room> query)
        {
            return query.Select(o => new Room { Code = o.Code });
        }

        public static bool Exists(this IQueryable<Room> query, string code)
        {
            return query.Any(o => o.Code == code);
        }

        public static IQueryable<Room> ExceptCodes(this IQueryable<Room> query, IEnumerable<string> codes)
        {
            return query.Where(q => !codes.Contains(q.Code));
        }

        public static IQueryable<Room> Codes(this IQueryable<Room> query, IEnumerable<string> codes)
        {
            return query.Where(q => codes.Contains(q.Code));
        }

        public static IQueryable<Room> NameContains(this IQueryable<Room> query, string nameContains)
        {
            return query.Where(o => o.Name.Contains(nameContains));
        }

        public static IQueryable<Room> Available(this IQueryable<Room> query, bool val)
        {
            return query.Where(o => o.IsAvailable == val);
        }

        public static IQueryable<Room> Archived(this IQueryable<Room> query, bool val)
        {
            return query.Where(o => o.Archived == val);
        }

        public static IQueryable<Room> NotHangingExcept(this IQueryable<Room> query, DateTime current, string hangingUserId)
        {
            return query.Where(o => o.HangingUserId == hangingUserId
                || o.HangingEndTime == null || o.HangingEndTime <= current);
        }

        public static IQueryable<Room> OfRoomType(this IQueryable<Room> query, string typeCode)
        {
            return query.Where(o => o.RoomTypeCode == typeCode);
        }

        public static IQueryable<Room> OfHangingUserId(this IQueryable<Room> query, string hangingUserId)
        {
            return query.Where(o => o.HangingUserId == hangingUserId);
        }

        public static IQueryable<Room> CanHandle(this IQueryable<Room> query, int numOfPeople)
        {
            return query.Where(o => o.PeopleCapacity >= numOfPeople);
        }

        public static IQueryable<Room> Search(this IQueryable<Room> query, string search)
        {
            return query.Where(o => o.Name.Contains(search) || o.Code.Contains(search)
                || o.RoomType.Name.Contains(search));
        }

        public static IQueryable<Room> InActiveTime(this IQueryable<Room> query, TimeSpan fromTime,
            TimeSpan toTime)
        {
            return query.Where(o => o.ActiveFromTime <= fromTime && o.ActiveToTime >= toTime);
        }

        public static IQueryable<Room> AvailableForBooking(this IQueryable<Room> query,
            string userId,
            IQueryable<Booking> bookingQuery,
            DateTime date, TimeSpan fromTime, TimeSpan toTime, int numOfPeople)
        {
            var now = DateTime.UtcNow;
            //empty room
            var notAvailableRoom = bookingQuery.ActiveStatus()
                .Overlapped(date, fromTime, toTime)
                .Select(b => b.Room);
            return query = query.Except(notAvailableRoom)
                .Available(true)
                .InActiveTime(fromTime, toTime)
                //not hanging by someone else
                .NotHangingExcept(now, userId)
                .CanHandle(numOfPeople);
        }

        #region Query
        public static async Task<IQueryable<Room>> FilterAsync(this IQueryable<Room> query, RoomQueryFilter model,
            string userId,
            IDictionary<string, object> tempData, IQueryable<Booking> bookingQuery)
        {
            var available = model.is_available ?? BoolOptions.B;
            if (available != BoolOptions.B)
                query = query.Available(available == BoolOptions.T);
            var archived = model.archived ?? BoolOptions.F;
            if (archived != BoolOptions.B)
                query = query.Archived(!(archived == BoolOptions.F));
            if (model.code != null)
                query = query.Code(model.code);
            if (model.name_contains != null)
                query = query.NameContains(model.name_contains);
            if (model.room_type != null)
                query = query.OfRoomType(model.room_type);
            if (model.num_of_people != null)
                query = query.CanHandle(model.num_of_people.Value);
            if (model.search != null)
                query = query.Search(model.search);
            if (model.empty)
            {
                //required fields
                var now = DateTime.UtcNow;
                var notAvailableRoomCode = bookingQuery.ActiveStatus()
                    .Overlapped(model.date.Value, model.from_time.Value, model.to_time.Value)
                    .Select(b => b.RoomCode).ToList();
                var dateLocal = model.date.Value.ToDefaultTimeZone();
                var fapBookedRoomInDate = await Global.FapClient.GetFAPScheduleInDateRangeAsync(
                    dateLocal, dateLocal);
                var fapNotAvailableRoom = fapBookedRoomInDate.AsQueryable()
                    .ActiveStatus()
                    .Overlapped(model.date.Value, model.from_time.Value, model.to_time.Value)
                    .Select(b => b.RoomCode).ToList();
                notAvailableRoomCode = notAvailableRoomCode.Union(fapNotAvailableRoom).ToList();
                query = query.ExceptCodes(notAvailableRoomCode)
                    .InActiveTime(model.from_time.Value, model.to_time.Value)
                    //not hanging by someone else
                    .NotHangingExcept(now, userId);

                //Already filter below
                //.Where(o => o.RoomTypeCode == model.room_type)
                //.Where(o => o.PeopleCapacity >= model.num_of_people);
            }
            return query;
        }

        public static IQueryable<Room> Sort(this IQueryable<Room> query, RoomQuerySort model)
        {
            foreach (var s in model._sortsArr)
            {
                var asc = s[0] == 'a';
                var fieldName = s.Remove(0, 1);
                switch (fieldName)
                {
                    case RoomQuerySort.CODE:
                        {
                            if (asc) query = query.OrderBy(o => o.Code);
                            else query = query.OrderByDescending(o => o.Code);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Room> Project(this IQueryable<Room> query, RoomQueryProjection model)
        {
            bool res = false, area = false, block = false, dep = false, level = false, roomType = false;
            foreach (var f in model.GetFieldsArr())
            {
                switch (f)
                {
                    case RoomQueryProjection.RESOURCES: res = true; break;
                    case RoomQueryProjection.AREA: area = true; break;
                    case RoomQueryProjection.BLOCK: block = true; break;
                    case RoomQueryProjection.DEPARTMENT: dep = true; break;
                    case RoomQueryProjection.LEVEL: level = true; break;
                    case RoomQueryProjection.ROOM_TYPE: roomType = true; break;
                }
            }
            query = query.Select(o => new Room
            {
                Archived = o.Archived,
                AreaSize = o.AreaSize,
                //Booking = o.Booking,
                BuildingArea = area ? o.BuildingArea : null,
                BuildingAreaCode = o.BuildingAreaCode,
                BuildingBlockCode = o.BuildingBlockCode,
                BuildingLevel = level ? new BuildingLevel
                {
                    Code = o.BuildingLevel.Code,
                    Name = o.BuildingLevel.Name,
                    BuildingBlock = block ? o.BuildingLevel.BuildingBlock : null
                } : (block ? new BuildingLevel
                {
                    BuildingBlock = o.BuildingLevel.BuildingBlock
                } : null),
                BuildingLevelCode = o.BuildingLevelCode,
                ActiveFromTime = o.ActiveFromTime,
                ActiveToTime = o.ActiveToTime,
                HangingUserId = o.HangingUserId,
                Code = o.Code,
                Department = dep ? o.Department : null,
                DepartmentCode = o.DepartmentCode,
                Description = o.Description,
                HangingEndTime = o.HangingEndTime,
                HangingStartTime = o.HangingStartTime,
                IsAvailable = o.IsAvailable,
                Name = o.Name,
                Note = o.Note,
                PeopleCapacity = o.PeopleCapacity,
                RoomResource = res ? o.RoomResource : null,
                RoomType = roomType ? o.RoomType : null,
                RoomTypeCode = o.RoomTypeCode,
            });
            return query;
        }
        #endregion

    }
}
