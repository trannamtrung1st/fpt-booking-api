using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Helpers
{
    public static class NotiHelper
    {
        public static async Task<string> Notify(string topic,
            Notification noti,
            IReadOnlyDictionary<string, string> data = null)
        {
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
            IReadOnlyDictionary<string, string> data = null)
        {
            var messages = topics.Select(t => new Message
            {
                Data = data,
                Notification = noti,
                Topic = t
            }).ToList();
            var result = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
            return result;
        }

        public static async Task<string> SendData(string topic, IReadOnlyDictionary<string, string> data)
        {
            var result = await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {
                Topic = topic,
                Data = data
            });
            return result;
        }
    }
}
