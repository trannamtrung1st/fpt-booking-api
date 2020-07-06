using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FPTBooking.Business
{
    public enum AppResultCode
    {
        [Display(Name = "Unknown error")]
        UnknownError = 1,
        [Display(Name = "Success")]
        Success = 2,
        [Display(Name = "Fail validation")]
        FailValidation = 3,
        [Display(Name = "Not found")]
        NotFound = 4,
        [Display(Name = "Unsupported")]
        Unsupported = 5,
        [Display(Name = "Can not delete because of dependencies")]
        DependencyDeleteFail = 6,
        [Display(Name = "Unauthorized")]
        Unauthorized = 7,
        [Display(Name = "Username has already existed")]
        DuplicatedUsername = 8,
        [Display(Name = "Invalid email domain")]
        InvalidEmailDomain = 9

    }

    public class JWT
    {
        public const string ISSUER = "fptbooking1st";
        public const string AUDIENCE = "fptbooking1st";
        public const string SECRET_KEY = "ASDFOIPJJP812340-89ADSFPOUADSFH809-3152-798OHASDFHPOU1324-8ASDF";

        public const string REFRESH_ISSUER = "refresh_fptbooking1st";
        public const string REFRESH_AUDIENCE = "refresh_fptbooking1st";
        public const string REFRESH_SECRET_KEY = "FSPDIU2093T-ASDGPIOSDGPHASDG-EWRQWGWQEGWE-QWER-QWER13412-AQRQWR";
    }

    public static class AppClaimType
    {
        public const string UserName = "username";
        public const string PhotoUrl = "photo_url";
    }

    public static class AppOAuthScope
    {
        public const string ROLES = "roles";
    }

    public static class ApiEndpoint
    {
        public const string ROLE_API = "api/roles";
        public const string USER_API = "api/users";
        public const string BOOKING_API = "api/bookings";
        public const string ROOM_API = "api/rooms";
        public const string ROOM_TYPE_API = "api/room-types";
        public const string DEV_API = "api/dev";
        public const string ERROR = "error";
    }

    public static class AppTimeZone
    {
        private static readonly TimeZoneInfo _default = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public static IDictionary<string, TimeZoneInfo> Map =
            new Dictionary<string, TimeZoneInfo>()
            {
                { AppCulture.VI, _default },
                { AppCulture.EN, _default },
            };
    }

    public static class AllowedEmailDomains
    {
        public const string FPT_DOMAIN = "fpt.edu.vn";
    }

    public static class MemberTypeValues
    {
        public const string GENERAL = "General";
    }

}
