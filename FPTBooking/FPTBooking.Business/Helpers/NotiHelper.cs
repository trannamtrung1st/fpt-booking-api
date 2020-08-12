using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Helpers
{
    public static class NotiHelper
    {
        public const string CLICK_ACTION = "FLUTTER_NOTIFICATION_CLICK";

        public static async Task<string> Notify(string topic,
            Notification noti,
            Dictionary<string, string> data = null)
        {
            if (data != null) data["click_action"] = CLICK_ACTION;
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Topic = topic,
                Notification = noti,
                Data = data,
            });
            return result;
        }

        public static async Task<BatchResponse> Notify(IEnumerable<string> topics,
            Notification noti,
            Dictionary<string, string> data = null)
        {
            if (data != null) data["click_action"] = CLICK_ACTION;
            var messages = topics.Select(t => new Message
            {
                Data = data,
                Notification = noti,
                Topic = t
            }).ToList();
            var result = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
            return result;
        }

        public static async Task<string> SendData(string topic, Dictionary<string, string> data)
        {
            if (data != null) data["click_action"] = CLICK_ACTION;
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Topic = topic,
                Data = data
            });
            return result;
        }
    }
}
