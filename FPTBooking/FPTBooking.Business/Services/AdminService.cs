using FirebaseAdmin.Messaging;
using FPTBooking.Business.Helpers;
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
            var result = await NotiHelper.Notify(entity.Id, new Notification
            {
                Body = model.Body,
                ImageUrl = model.ImageUrl,
                Title = model.Title,
            }, model.Data);
            return result;
        }
    }
}
