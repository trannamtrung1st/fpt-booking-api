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

        public Room ToRoom()
        {
            var entity = new Room
            {
                Archived = false,
                AreaSize = null,
                BuildingAreaCode = AreaId == 4 ? "LUK" : "CR",
                Code = RoomNo,
                DepartmentCode = "DOE",
                Description = Description,
                Name = RoomNo,
                IsAvailable = !IsDisable,
                Note = null,
                RoomTypeCode = "CR",
                PeopleCapacity = Capacity,
            };
            entity.RoomResource = new List<RoomResource>
            {
                new RoomResource
                {
                    Code = "display-screen",
                    IsAvailable =true,
                    Name = "Display screen",
                    Room = entity,
                }
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
