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
        public double? AreaSize { get; set; }
        public int PeopleCapacity { get; set; }
        public DateTime? HangingStartTime { get; set; }
        public DateTime? HangingEndTime { get; set; }
        public TimeSpan ActiveFromTime { get; set; }
        public TimeSpan ActiveToTime { get; set; }
        public string HangingUserId { get; set; }
        public bool IsAvailable { get; set; }
        public string Note { get; set; }

        public virtual BuildingArea BuildingArea { get; set; }
        public virtual BuildingLevel BuildingLevel { get; set; }
        public virtual Department Department { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<Booking> Booking { get; set; }
        public virtual ICollection<RoomResource> RoomResource { get; set; }
    }
}
