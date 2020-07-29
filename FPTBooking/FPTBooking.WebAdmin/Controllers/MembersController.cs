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

    [Route(ApiEndpoint.MEMBER_API)]
    [ApiController]
    [InjectionFilter]
    public class MembersController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly IdentityService _identityService;
        [Inject]
        private readonly MemberService _service;
        [Inject]
        private readonly SystemService _sysService;

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpPost("")]
        public async Task<IActionResult> CreateMember(CreateMemberModel model)
        {
            bool checkEmailDomain = _identityService.ValidateEmailDomain(model.Email);
            if (!checkEmailDomain)
                return Unauthorized(AppResult.InvalidEmailDomain());
            var entity = await _identityService.GetUserByEmailAsync(model.Email);
            if (entity != null)
            {
                var validationData = new ValidationData();
                validationData = validationData.Fail(code: AppResultCode.EmailExisted);
                return BadRequest(AppResult.FailValidation(validationData));
            }
            var emailInfo = model.Email.GetEmailInfo();
            entity = _identityService.ConvertToUser(model, emailInfo.Item3);
            using (var transaction = context.Database.BeginTransaction())
            {
                model.Roles = model.Roles ?? new HashSet<string>();
                if (model.CreateDepartmentMembers.Any(o => o.IsManager == true))
                    model.Roles.Add(RoleName.MANAGER);
                var result = await _identityService
                    .CreateUserWithoutPassAsync(entity, model.Roles);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError(err.Code, err.Description);
                    var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
                    return BadRequest(builder);
                }
                _logger.CustomProperties(entity).Info("Register new user");
                var memberEntity = _service.CreateMember(model, entity, emailInfo);
                //log event
                var ev = _sysService.GetEventForNewUser(
                    $"Admin has created a user with email {model.Email}",
                    UserId);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                transaction.Commit();
            }
            return NoContent();
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var entity = _service.Members.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteMember(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                using (var trans = context.Database.BeginTransaction())
                {
                    var user = entity.User;
                    _service.DeleteMemberTransaction(entity);
                    var result = await _identityService.DeleteUserAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                            ModelState.AddModelError(err.Code, err.Description);
                        var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
                        return BadRequest(builder);
                    }
                    //log event
                    var ev = _sysService.GetEventForDeleteMember(
                        $"Admin has remove a user with email {entity.Email}",
                        UserId);
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
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, UpdateMemberModel model)
        {
            var entity = _service.Members.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateMember(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using (var transaction = context.Database.BeginTransaction())
            {
                var user = entity.User;
                _service.UpdateMember(entity, model);
                user = _identityService.UpdateUser(user, model);
                if (model.UpdateDepartmentMembers.Any(o => o.IsManager == true))
                {
                    model.Roles = model.Roles ?? new HashSet<string>();
                    model.Roles.Add(RoleName.MANAGER);
                }
                var result = await _identityService.UpdateUserAsync(user);
                if (result.Succeeded && model.Roles != null && model.Roles.Count() > 0)
                {
                    var oldRoles = await _identityService.GetRolesOfUserAsync(user);
                    result = await _identityService.RemoveUserFromRolesAsync(user, oldRoles);
                    if (result.Succeeded)
                        result = await _identityService.AddRolesForUserAsync(user, model.Roles);
                }
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError(err.Code, err.Description);
                    var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
                    return BadRequest(builder);
                }
                _logger.CustomProperties(User).Info("Update user");
                //log event
                var ev = _sysService.GetEventForUpdateUser(
                    $"Admin has updated a user information",
                    UserId);
                _sysService.CreateAppEvent(ev);
                //end log event
                context.SaveChanges();
                transaction.Commit();
            }
            return NoContent();
        }

#if !DEBUG
        [Authorize(Roles = RoleName.ADMIN)]
#else
        [Authorize]
#endif
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]MemberQueryFilter filter,
            [FromQuery]MemberQuerySort sort,
            [FromQuery] Business.Models.MemberQueryProjection projection,
            [FromQuery]MemberQueryPaging paging,
            [FromQuery]MemberQueryOptions options)
        {
            var validationData = _service.ValidateGetMembers(
               filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryMembersDynamic(
                projection, validationData.TempData, filter, sort, paging, options);
            if (options.single_only && result == null) return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(data: result));
        }

    }
}