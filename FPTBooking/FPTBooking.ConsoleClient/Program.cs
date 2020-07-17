using FPTBooking.Business.Clients;
using FPTBooking.Business.Clients.Models;
using FPTBooking.Business.Helpers;
using FPTBooking.Data;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPTBooking.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = "Khanht@fpt.edu.vn";
            var code = test.GetTeacherCode();
            Console.WriteLine(code);
            using (var fapClient = new FptFapClient("http://fap.fpt.edu.vn", "fap-secret-key"))
            {
                //var test = fapClient.GetScheduleInDateRangeAsync(
                //    new DateTime(2020, 7, 7),
                //    new DateTime(2020, 7, 10),
                //    "KhanhT").Result;

                //var test = fapClient.GetAllSlots().Result;
                //fapClient.CacheData().Wait();
                //var test = fapClient.GetActivityTeacher(DateTime.Now, "KhanhT").Result;
                //test = fapClient.GetActivityStudent(DateTime.Now, "SE130097").Result;
                //InsertRooms(fapClient);
            }
        }

        static void InsertRooms(FptFapClient fapClient)
        {
            var fapRooms = fapClient.GetAllRooms().Result;
            var rooms = fapRooms.Select(o => o.ToRoom())
                .Distinct(new RoomComparer()).ToList();
            var builder = new DbContextOptionsBuilder<DataContext>().UseSqlServer(
                DataConsts.CONN_STR);
            var options = builder.Options;
            using (var context = new DataContext(options))
            {
                context.Room.AddRange(rooms);
                context.SaveChanges();
            }
        }
    }
}
