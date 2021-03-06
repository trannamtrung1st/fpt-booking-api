﻿using System;
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
using FPTBooking.Business.Helpers;

namespace FPTBooking.WebAdmin.Controllers
{

    [Route(ApiEndpoint.BOOKING_SERVICE_API)]
    [ApiController]
    [InjectionFilter]
    public class BookingServicesController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly BookingServiceBusinessService _service;

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]BookingServiceQueryFilter filter,
            [FromQuery]BookingServiceQuerySort sort,
            [FromQuery]BookingServiceQueryProjection projection,
            [FromQuery]BookingServiceQueryPaging paging,
            [FromQuery]BookingServiceQueryOptions options)
        {
            var validationData = _service.ValidateGetBookingServices(
               filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryBookingServiceDynamic(
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

    }
}