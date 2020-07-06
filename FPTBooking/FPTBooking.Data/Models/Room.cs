using System;
using System.Collections.Generic;

namespace FPTBooking.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Booking = new HashSet<Booking>();
            RoomResource = new HashSet<RoomResource>();
            RoomService = new HashSet<RoomService>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }
        public string RoomTypeCode { get; set; }
        public string BuildingAreaCode { get; set; }
        public string BuildingLevelCode { get; set; }
        public string BuildingBlockCode { get; set; }
        public string DepartmentCode { get; set; }
        public double AreaSize { get; set; }
        public int PeopleCapacity { get; set; }
        public TimeSpan HangingStartTime { get; set; }
        public TimeSpan HangingEndTime { get; set; }
        public int Status { get; set; }

        public virtual BuildingArea BuildingAreaCodeNavigation { get; set; }
        public virtual BuildingLevel BuildingLevelCodeNavigation { get; set; }
        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual RoomType RoomTypeCodeNavigation { get; set; }
        public virtual ICollection<Booking> Booking { get; set; }
        public virtual ICollection<RoomResource> RoomResource { get; set; }
        public virtual ICollection<RoomService> RoomService { get; set; }
    }
}
