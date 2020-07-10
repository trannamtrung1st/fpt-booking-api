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

    public static class MemberTypeName
    {
        public const string TEACHER = "Teacher";
        public const string STUDENT = "Student";
        public const string GENERAL = "Employee";
    }


}
