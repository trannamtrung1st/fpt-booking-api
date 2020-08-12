using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPTBooking.WebHelpers;

namespace FPTBooking.WebAdmin
{
    public static class Routing
    {
        public const string ADMIN = "/admin";
        public const string ROOM = "/room";
        public const string AREA = "/buildingarea";
        public const string DEPARTMENT = "/department";
        public const string USER = "/user";
        public const string DASHBOARD = "/dashboard";
        public const string LOGIN = "/identity/login";
        public const string LOGOUT = "/identity/logout";
        public const string IDENTITY = "/identity";
        public const string ADMIN_ONLY = "/adminonly";
        public const string ACCESS_DENIED = "/accessdenied";
        public const string STATUS = "/status";
        public const string ERROR = "/error";
        public const string ERROR_CONTROLLER = "error";
        public const string INDEX = "/";
    }

    public static class AppCookie
    {
        public const string TOKEN = "_appuat";
    }

    public static class AppView
    {
        public const string MESSAGE = "MessageView";
        public const string STATUS = "StatusView";
        public const string ERROR = "ErrorView";
    }

    public static class Menu
    {
        public const string DASHBOARD = "dashboard";
        public const string USER = "user";
        public const string DEPARTMENT = "department";
        public const string AREA = "area";
        public const string ROOM = "room";
        public const string ADMIN = "admin";
    }

}
