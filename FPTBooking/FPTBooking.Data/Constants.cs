using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FPTBooking.Data
{

    public class DataConsts
    {
        public const string CONN_STR = "Server=localhost;Database=FPTBooking;Trusted_Connection=False;User Id=sa;Password=123456;MultipleActiveResultSets=true";
    }

    public static class RoleName
    {
        public const string ADMIN = "Administrator";
        public const string MANAGER = "Manager";
        public const string USER = "User";
        public const string ROOM_CHECKER = "RoomChecker";
    }

    public static class BuildingLevelValues
    {
        public static readonly BuildingLevel L1 = new BuildingLevel
        {
            Archived = false,
            BuildingBlockCode = BuildingBlockValues.MAIN.Code,
            Code = "F1",
            Name = "1st floor",
        };
        public static readonly BuildingLevel L2 = new BuildingLevel
        {
            Archived = false,
            BuildingBlockCode = BuildingBlockValues.MAIN.Code,
            Code = "F2",
            Name = "2nd floor",
        };
        public static readonly BuildingLevel L3 = new BuildingLevel
        {
            Archived = false,
            BuildingBlockCode = BuildingBlockValues.MAIN.Code,
            Code = "F3",
            Name = "3rd floor",
        };
        public static readonly BuildingLevel L4 = new BuildingLevel
        {
            Archived = false,
            BuildingBlockCode = BuildingBlockValues.MAIN.Code,
            Code = "F4",
            Name = "4th floor",
        };
    }

    public static class BuildingBlockValues
    {
        public static readonly BuildingBlock MAIN = new BuildingBlock
        {
            Archived = false,
            Code = "MAIN",
            Name = "Main building"
        };
    }

    public static class BuildingAreaValues
    {
        public static readonly BuildingArea LIBRARY = new BuildingArea
        {
            Archived = false,
            Code = "LB",
            Description = "Library",
            Name = "Library",
        };

        public static readonly BuildingArea ADMIN = new BuildingArea
        {
            Archived = false,
            Code = "ADMIN",
            Description = "Administrative",
            Name = "Administrative",
        };
    }

    public static class DeparmentValues
    {
        public static readonly Department LIBRARY = new Department
        {
            Archived = false,
            Code = "LB",
            Description = "Library",
            Name = "Library",
        };

        public static readonly Department ADMIN = new Department
        {
            Archived = false,
            Code = "Administrative",
            Description = "Administrative",
            Name = "Administrative",
        };
    }

    public static class RoomTypeValues
    {
        public static readonly RoomType LIBRARY = new RoomType
        {
            Archived = false,
            Code = "LB",
            Description = "Library",
            Name = "Library",
        };

        public static readonly RoomType ADMIN = new RoomType
        {
            Archived = false,
            Code = "ADMIN",
            Description = "Administrative",
            Name = "Administrative",
        };
    }

    public static class UserValues
    {
        public static readonly string LIB_EMAIL = "TraPTP@fpt.edu.vn";
        public static readonly string ADMIN_EMAIL = "utnt@fpt.edu.vn";
    }
}
