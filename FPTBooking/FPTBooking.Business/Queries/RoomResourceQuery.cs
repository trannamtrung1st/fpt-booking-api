using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Queries
{
    public static class RoomResourceQuery
    {
        public static IQueryable<RoomResource> OfRoom(this IQueryable<RoomResource> query, string code)
        {
            return query.Where(o => o.RoomCode == code);
        }

    }
}
