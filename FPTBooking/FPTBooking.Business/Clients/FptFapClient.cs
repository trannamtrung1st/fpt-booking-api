using FPTBooking.Business.Helpers;
using FPTBooking.Business.Clients.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using FPTBooking.Data.Models;
using System.Security.Claims;
using FPTBooking.Data;
using System.Linq;

namespace FPTBooking.Business.Clients
{
    /// <summary>
    /// This class need updating if FAP service is updated
    /// </summary>
    public class FptFapClient : IDisposable
    {
        public IDictionary<string, ValueTuple<TimeSpan, TimeSpan>> SlotMap { get; set; }
            = new Dictionary<string, ValueTuple<TimeSpan, TimeSpan>>();

        protected readonly HttpClient http;
        protected readonly string key;
        public FptFapClient(string baseAddress, string key)
        {
            this.key = key;
            http = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress, UriKind.Absolute),
            };
        }

        public async Task CacheData()
        {
            var slots = await GetAllSlots();
            foreach (var o in slots)
            {
                SlotMap[o.Slot.ToString()] = (new TimeSpan(o.StartHour, o.StartMinute, 0),
                    new TimeSpan(o.EndHour, o.EndMinute, 0));
            }
        }

        public async Task<IEnumerable<FAPSlot>> GetAllSlots()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = this.key;
            string queryString = query.ToString();
            var respStr = await http.GetStringAsync($"/Slotview.asmx/GetAllSlot?{queryString}");
            XDocument doc = XDocument.Parse(respStr);
            return JsonConvert.DeserializeObject<IEnumerable<FAPSlot>>(doc.Element(
                XName.Get("string", "http://tempuri.org/")).Value);
        }

        public async Task<IEnumerable<FAPRoom>> GetAllRooms()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = this.key;
            string queryString = query.ToString();
            var respStr = await http.GetStringAsync($"/Slotview.asmx/GetOneAllRoome?{queryString}");
            XDocument doc = XDocument.Parse(respStr);
            return JsonConvert.DeserializeObject<IEnumerable<FAPRoom>>(doc.Element(
                XName.Get("string", "http://tempuri.org/")).Value);
        }

        public async Task<IEnumerable<FAPSchedule>> GetScheduleInDateRangeAsync(DateTime fromDate,
            DateTime toDate, string lectureCode)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = this.key;
            query["fDate"] = fromDate.ToString(dateFormat: "MM/dd/yyyy");
            query["eDate"] = toDate.ToString(dateFormat: "MM/dd/yyyy");
            query["lecture"] = lectureCode;
            string queryString = query.ToString();
            var respStr = await http.GetStringAsync($"/Slotview.asmx/GetScheduleFromDate?{queryString}");
            XDocument doc = XDocument.Parse(respStr);
            return JsonConvert.DeserializeObject<IEnumerable<FAPSchedule>>(doc.Element(
                XName.Get("string", "http://tempuri.org/")).Value);
        }

        public async Task<IEnumerable<FAPActivity>> GetActivityTeacher(
            DateTime date, string lecturer)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = this.key;
            query["date"] = date.ToString(dateFormat: "MM/dd/yyyy");
            query["TeacherCode"] = lecturer;
            string queryString = query.ToString();
            var respStr = await http.GetStringAsync($"/Slotview.asmx/GetActivityTeacher?{queryString}");
            XDocument doc = XDocument.Parse(respStr);
            return JsonConvert.DeserializeObject<IEnumerable<FAPActivity>>(doc.Element(
                XName.Get("string", "http://tempuri.org/")).Value);
        }

        public async Task<IEnumerable<FAPActivity>> GetActivityStudent(
            DateTime date, string rollNumber)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = this.key;
            query["date"] = date.ToString(dateFormat: "MM/dd/yyyy");
            query["RollNumber"] = rollNumber;
            string queryString = query.ToString();
            var respStr = await http.GetStringAsync($"/Slotview.asmx/GetActivityStudent?{queryString}");
            XDocument doc = XDocument.Parse(respStr);
            return JsonConvert.DeserializeObject<IEnumerable<FAPActivity>>(doc.Element(
                XName.Get("string", "http://tempuri.org/")).Value);
        }

        public async Task<List<Booking>> GetFAPOwnerBookingAsync(ClaimsPrincipal principal,
            Member member,
            DateTime date)
        {
            var list = new List<Booking>();
            if (member.MemberType.Name == MemberTypeName.STUDENT)
            {
                var resp = await Global.FapClient.GetActivityStudent(date, member.Code);
                list = resp.Select(o => o.ToBooking()).ToList();
            }
            else if (member.MemberType.Name == MemberTypeName.TEACHER)
            {
                var resp = await Global.FapClient.GetActivityTeacher(date, member.Code);
                list = resp.Select(o => o.ToBooking()).ToList();
            }
            return list;
        }

        public async Task<List<Booking>> GetFAPScheduleInDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var list = await GetScheduleInDateRangeAsync(fromDate, toDate, null);
            return list.Select(o => o.ToBooking(SlotMap)).ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                http.Dispose();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FptFapClient()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
