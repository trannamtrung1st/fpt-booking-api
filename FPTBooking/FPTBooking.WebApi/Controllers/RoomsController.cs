using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using FPTBooking.Business;
using FPTBooking.Business.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.General;
using FPTBooking.Business.Services;

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.ROOM_API)]
    [ApiController]
    [InjectionFilter]
    public class RoomsController : BaseController
    {

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly RoomBusinessService _service;

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]RoomQueryFilter filter,
            [FromQuery]RoomQuerySort sort,
            [FromQuery]RoomQueryProjection projection,
            [FromQuery]RoomQueryPaging paging,
            [FromQuery]RoomQueryOptions options)
        {
            if (Settings.Instance.Mocking.Enabled || true)
            {
                var rd = new Random();
                Func<string> randomCode = () =>
                    rd.RandomStringFrom(RandomExtension.Uppers_Digits, 4);
                var list = new List<object>
                {
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                    new
                    {
                        code = randomCode(),
                        name = randomCode(),
                        area_size = rd.Next(10, 30),
                        people_capacity= rd.Next(10, 30),
                        room_type = new
                        {
                            code = "Classroom",
                            name = "Classroom"
                        },
                        is_available = true,
                    },
                };
                list.AddRange(list);
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return Ok(AppResult.Success(data: new
                        {
                            list = list
                        }));
                    case 5:
                    case 6:
                        throw new Exception("Test exception");
                }
            }
            var validationData = _service.ValidateGetRooms(
                filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryRoomDynamic(
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }


        [HttpGet("{code}")]
        public IActionResult GetDetail(string code)
        {
            if (Settings.Instance.Mocking.Enabled || true)
            {
                var rd = new Random();
                var randomCode = rd.RandomStringFrom(RandomExtension.Uppers_Digits, 4);
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return Ok(AppResult.Success(data: new
                        {
                            single = new
                            {
                                code = randomCode,
                                name = randomCode,
                                area_size = rd.Next(10, 30),
                                people_capacity = rd.Next(10, 30),
                                description = "This is the room description",
                                room_type = new
                                {
                                    code = "CR",
                                    name = "Classroom"
                                },
                                block = new
                                {
                                    code = "B1",
                                    name = "Block 1"
                                },
                                level = new
                                {
                                    code = "F1",
                                    name = "Floor 1"
                                },
                                is_available = true,
                                area = new
                                {
                                    code = "CR",
                                    name = "Classroom"
                                },
                                department = new
                                {
                                    code = "D1",
                                    name = "Department of Education"
                                },
                                resources = new List<object>
                            {
                                new
                                {
                                    code = "PRJ",
                                    name = "Projector",
                                    is_available = true,
                                },
                                new
                                {
                                    code = "SCR",
                                    name = "Display screen",
                                    is_available = false,
                                }
                            },
                                available_services = new List<object>
                            {
                                new
                                {
                                    code = "PRJ",
                                    name = "Projector"
                                },
                                new
                                {
                                    code = "MS",
                                    name = "Mentor/Support"
                                }
                            },
                                note = "This is note from Room checker"
                            }
                        }));
                    case 5:
                    case 6:
                        throw new Exception("Test exception");
                }
            }
            throw new NotImplementedException();
        }

    }
}