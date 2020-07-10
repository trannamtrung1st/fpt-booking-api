using FPTBooking.FAPClient;
using System;

namespace FPTBooking.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var fapClient = new FptFapClient("http://fap.fpt.edu.vn"))
            {
                //var test = fapClient.GetScheduleInDateRangeAsync(
                //    new DateTime(2020, 7, 7),
                //    new DateTime(2020, 7, 10),
                //    "KhanhT").Result;

                //var test = fapClient.GetAllSlot().Result;
                fapClient.CacheData().Wait();
                var test = fapClient.GetActivityTeacher(DateTime.Now, "KhanhT").Result;
                test = fapClient.GetActivityStudent(DateTime.Now, "SE130097").Result;
            }
        }
    }
}
