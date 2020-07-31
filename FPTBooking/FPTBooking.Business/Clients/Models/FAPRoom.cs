using FPTBooking.Data;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Clients.Models
{
    public class FAPRoom
    {
        public int AreaId { get; set; }
        public string RoomNo { get; set; }
        public int Capacity { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; }
        public bool IsDisable { get; set; }

        protected RoomResource GetNewRoomResForRoom(string code, string name, string roomCode)
        {
            return new RoomResource
            {
                Code = code,
                Name = name,
                RoomCode = roomCode,
                IsAvailable = true
            };
        }

        public Room ToRoom()
        {
            if (AreaId != 3)
                throw new Exception("Only area 3 supported");
            var entity = new Room
            {
                Archived = false,
                AreaSize = null,
                BuildingAreaCode = BuildingAreaValues.ADMIN.Code,
                Code = RoomNo,
                DepartmentCode = DeparmentValues.ADMIN.Code,
                Description = Description,
                Name = RoomNo,
                IsAvailable = !IsDisable,
                Note = null,
                RoomTypeCode = RoomTypeValues.ADMIN.Code,
                PeopleCapacity = Capacity,
                ActiveFromTime = new TimeSpan(7, 0, 0),
                ActiveToTime = new TimeSpan(22, 0, 0),
            };
            entity.RoomResource = new List<RoomResource>
            {
                GetNewRoomResForRoom("AC", "Air-conditioner", RoomNo),
                GetNewRoomResForRoom("FURNITURE", "Furniture", RoomNo)
            };
            return entity;
        }
    }

    public class RoomComparer : IEqualityComparer<Room>
    {
        public bool Equals([AllowNull] Room x, [AllowNull] Room y)
        {
            return x?.Code == y?.Code;
        }

        public int GetHashCode([DisallowNull] Room obj)
        {
            return obj.Code.GetHashCode();
        }
    }
}
