using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AppEvent GetEventForBookingProcessing(BookingHistory history)
        {
            return new AppEvent
            {
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
                DisplayContent = displayContent,
                Data = JsonConvert.SerializeObject(data),
                HappenedTime = DateTime.UtcNow,
                Type = "NewUser",
                UserId = userId,
            };
        }

    }
}
