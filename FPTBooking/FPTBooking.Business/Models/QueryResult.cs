using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Models
{
    public class QueryResult<T>
    {
        [JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> Results { get; set; }
        [JsonProperty("single", NullValueHandling = NullValueHandling.Ignore)]
        public T SingleResult { get; set; }
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalCount { get; set; }
    }
}
