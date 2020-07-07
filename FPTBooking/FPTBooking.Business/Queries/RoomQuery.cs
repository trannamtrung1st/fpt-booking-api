﻿using FPTBooking.Business.Models;
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

        public static IQueryable<Room> NotHanging(this IQueryable<Room> query, DateTime current)
        {
            return query.Where(o => o.HangingEndTime <= current);
        }

        public static IQueryable<Room> OfRoomType(this IQueryable<Room> query, string typeCode)
        {
            return query.Where(o => o.RoomTypeCode == typeCode);
        }

        public static IQueryable<Room> CanHandle(this IQueryable<Room> query, int numOfPeople)
        {
            return query.Where(o => o.PeopleCapacity >= numOfPeople);
        }

        #region Query
        public static IQueryable<Room> Filter(this IQueryable<Room> query, RoomQueryFilter model,
            IDictionary<string, object> tempData, IQueryable<Booking> bookingQuery)
        {
            var available = model.available ?? BoolOptions.B;
            if (available != BoolOptions.B)
                query = query.Available(available == BoolOptions.T);
            var archived = model.available ?? BoolOptions.F;
            if (archived != BoolOptions.B)
                query = query.Archived(!(archived == BoolOptions.F));
            if (model.empty)
            {
                //required fields
                var date = (DateTime)tempData["date"];
                var fromTime = (TimeSpan)tempData["from_time"];
                var toTime = (TimeSpan)tempData["to_time"];
                var now = DateTime.Now;
                //empty room
                var notAvailableRoom = bookingQuery.ActiveStatus()
                    .Overlapped(date, fromTime, toTime)
                    .Select(b => b.Room);
                query = query.Except(notAvailableRoom)
                    //not hanging by someone else
                    .NotHanging(now);

                //Already filter below
                //.Where(o => o.RoomTypeCode == model.room_type)
                //.Where(o => o.PeopleCapacity >= model.num_of_people);
            }
            if (model.code != null)
                query = query.Code(model.code);
            if (model.name_contains != null)
                query = query.NameContains(model.name_contains);
            if (model.room_type != null)
                query = query.OfRoomType(model.room_type);
            if (model.num_of_people != null)
                query = query.CanHandle(model.num_of_people.Value);
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
                RoomResource = res ? o.RoomResource.Where(r => r.IsAvailable).ToList() : null,
                RoomType = roomType ? o.RoomType : null,
                RoomTypeCode = o.RoomTypeCode,
            });
            return query;
        }
        #endregion

    }
}
