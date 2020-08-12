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
        InvalidEmailDomain = 9,
        [Display(Name = "Access denied")]
        AccessDenied = 10,
        [Display(Name = "Email existed")]
        EmailExisted = 11
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
        public const string MemberCode = "member_code";
    }

    public static class AppOAuthScope
    {
        public const string ROLES = "roles";
    }

    public static class ApiEndpoint
    {
        public const string AREA_API = "api/areas";
        public const string DEPARTMENT_API = "api/departments";
        public const string ROLE_API = "api/roles";
        public const string USER_API = "api/users";
        public const string MEMBER_API = "api/members";
        public const string BOOKING_API = "api/bookings";
        public const string ROOM_API = "api/rooms";
        public const string ROOM_TYPE_API = "api/room-types";
        public const string BOOKING_SERVICE_API = "api/booking-services";
        public const string ADMIN_API = "api/admin";
        public const string ERROR = "error";
    }

    public static class AppDateTimeFormat
    {
        public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";
        public const string DEFAULT_FORMAT_FOR_CONVERT = "dd/MM/yyyy HH:mm";
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

    public static class BookingTypeValues
    {
        public const string BOOKING = "Booking";
        public const string SCHEDULE = "Schedule";
    }

    public static class BookingStatusValues
    {
        public const string PROCESSING = "Processing";
        public const string VALID = "Valid";
        public const string APPROVED = "Approved";
        public const string DENIED = "Denied";
        public const string ABORTED = "Aborted";
        public const string FINISHED = "Finished"; //after feedback

        public static readonly IEnumerable<string> ALL = new List<string>
        {
            PROCESSING, VALID, APPROVED, DENIED, ABORTED, FINISHED
        };
    }

    public static class BookingHistoryTypes
    {
        public const string CREATE = "Booking_Create";
        public const string APPROVE = "Booking_Approve";
        public const string FEEDBACK = "Booking_Feedback";
        public const string DENY = "Booking_Deny";
        public const string ABORT = "Booking_Abort";
        public const string UPDATE = "Booking_Update";
    }

    public enum BookingPrincipalRelationship
    {
        Manager = 1,
        Owner = 2
    }

    public enum BoolOptions
    {
        T = 1, F = 2, B = 3
    }

    public class BookingQueryFilterDateType
    {
        public const string BOOKED_DATE = "booked";
        public const string SENT_DATE = "sent";
    }

}
