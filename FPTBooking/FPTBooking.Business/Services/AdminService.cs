using FirebaseAdmin.Messaging;
using FPTBooking.Business.Models;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class AdminService : Service
    {
        public AdminService(ServiceInjection inj) : base(inj)
        {
        }

        public async Task<string> PushNotiToUser(AppUser entity, PushNotiModel model)
        {
            var mess = new Message
            {
                Data = model.Data,
                Notification = new Notification
                {
                    Body = model.Body,
                    ImageUrl = model.ImageUrl,
                    Title = model.Title,
                },
                Topic = entity.Id
            };
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(mess);
            return result;
        }
    }
}
