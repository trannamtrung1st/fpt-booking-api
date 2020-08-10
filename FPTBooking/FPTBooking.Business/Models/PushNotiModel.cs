using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Models
{
    public class PushNotiModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; }
    }

    public class ConfigModel
    {
        [JsonProperty("student_allowed")]
        public bool StudentAllowed { get; set; }
    }
}
