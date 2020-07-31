using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class SystemService : Service
    {
        public SystemService(ServiceInjection inj) : base(inj)
        {
        }

        public AppEvent CreateAppEvent(AppEvent ev)
        {
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent GetEventForCreateRoom(string display, ClaimsPrincipal principal, Room entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Name,
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "CreateRoom",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForDeleteRoom(string display, ClaimsPrincipal principal, Room entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "DeleteRoom",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForSyncRoomWithFap(string display, ClaimsPrincipal principal)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = null,
                HappenedTime = DateTime.UtcNow,
                Type = "SyncRoomWithFap",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForCreateBuildingArea(string display, ClaimsPrincipal principal, BuildingArea entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Name,
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "CreateBuildingArea",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForDeleteBuildingArea(string display, ClaimsPrincipal principal, BuildingArea entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "DeleteBuildingArea",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForUpdateBuildingArea(string display, ClaimsPrincipal principal, UpdateBuildingAreaModel model)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(model),
                HappenedTime = DateTime.UtcNow,
                Type = "UpdateBuildingArea",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForCreateDepartment(string display, ClaimsPrincipal principal, Department entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Name,
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "CreateDepartment",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForDeleteDepartment(string display, ClaimsPrincipal principal, Department entity)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(new
                {
                    entity.Code
                }),
                HappenedTime = DateTime.UtcNow,
                Type = "DeleteDepartment",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForUpdateDepartment(string display, ClaimsPrincipal principal, UpdateDepartmentModel model)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = display,
                Data = JsonConvert.SerializeObject(model),
                HappenedTime = DateTime.UtcNow,
                Type = "UpdateDepartment",
                UserId = principal.Identity.Name
            };
        }

        public AppEvent GetEventForBookingProcessing(BookingHistory history)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = history.DisplayContent,
                Data = history.Data,
                HappenedTime = history.HappenedTime,
                Type = history.Type,
                UserId = history.MemberId
            };
        }

        public AppEvent GetEventForRoomProcessing(string displayContent, string type, string userId,
            object data = null)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = displayContent,
                Data = JsonConvert.SerializeObject(data),
                HappenedTime = DateTime.UtcNow,
                Type = type,
                UserId = userId,
            };
        }

        public AppEvent GetEventForNewUser(string displayContent,
            string userId,
            object data = null)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = displayContent,
                Data = JsonConvert.SerializeObject(data),
                HappenedTime = DateTime.UtcNow,
                Type = "NewUser",
                UserId = userId,
            };
        }

        public AppEvent GetEventForUpdateUser(string displayContent,
            string userId,
            object data = null)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = displayContent,
                Data = JsonConvert.SerializeObject(data),
                HappenedTime = DateTime.UtcNow,
                Type = "UpdateUser",
                UserId = userId,
            };
        }

        public AppEvent GetEventForDeleteMember(string displayContent,
            string userId,
            object data = null)
        {
            return new AppEvent
            {
                Id = Guid.NewGuid().ToString(),
                DisplayContent = displayContent,
                Data = JsonConvert.SerializeObject(data),
                HappenedTime = DateTime.UtcNow,
                Type = "DeleteUser",
                UserId = userId,
            };
        }

    }
}
