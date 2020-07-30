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
using FPTBooking.Business.Services;
using FPTBooking.Data.Models;
using FirebaseAdmin.Auth;
using FPTBooking.Data.Helpers;
using FPTBooking.Business.Helpers;
using FPTBooking.Data;
using FPTBooking.Business.Queries;
using FPTBooking.WebHelpers;
using Microsoft.EntityFrameworkCore;

namespace FPTBooking.WebAdmin.Controllers
{

    [Route(ApiEndpoint.AREA_API)]
    [ApiController]
    [InjectionFilter]
    public class AreasController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly AreaService _service;
        [Inject]
        private readonly SystemService _sysService;

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]AreaQueryFilter filter,
            [FromQuery]AreaQuerySort sort,
            [FromQuery] Business.Models.AreaQueryProjection projection,
            [FromQuery]AreaQueryPaging paging,
            [FromQuery]AreaQueryOptions options)
        {
            var validationData = _service.ValidateGetAreas(
               filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryAreaDynamic(
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }


#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpDelete("{code}")]
        public IActionResult Delete(string code)
        {
            try
            {
                var entity = _service.BuildingAreas.Code(code).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteBuildingArea(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                using (var trans = context.Database.BeginTransaction())
                {
                    _service.DeleteBuildingArea(entity);
                    //log event
                    var ev = _sysService.GetEventForDeleteBuildingArea(
                        $"Admin {UserEmail} deleted area {entity.Name}", User,
                        entity);
                    _sysService.CreateAppEvent(ev);
                    //end log event
                    context.SaveChanges();
                    trans.Commit();
                }
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(AppResult.FailValidation(data: new ValidationData()
                    .Fail(code: AppResultCode.DependencyDeleteFail)));
            }
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPost("")]
        public IActionResult CreateBuildingArea(CreateBuildingAreaModel model)
        {
            var validationData = _service.ValidateCreateBuildingArea(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var trans = context.Database.BeginTransaction())
            {
                var entity = _service.CreateBuildingArea(model);
                //log event
                var ev = _sysService.GetEventForCreateBuildingArea(
                    $"Admin {UserEmail} created a new area", User, entity);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                trans.Commit();
            }
            return NoContent();
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPatch("{code}")]
        public IActionResult Update(string code, UpdateBuildingAreaModel model)
        {
            var entity = _service.BuildingAreas.Code(code).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateBuildingArea(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var transaction = context.Database.BeginTransaction())
            {
                _service.UpdateBuildingArea(entity, model);
                //log event
                var ev = _sysService.GetEventForUpdateBuildingArea(
                    $"Admin {UserEmail} updated area {entity.Name}",
                    User, model);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                transaction.Commit();
            }
            return NoContent();
        }
    }
}