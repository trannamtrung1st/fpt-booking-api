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
using FPTBooking.WebApi.Helpers;
using FPTBooking.Data.Helpers;

namespace FPTBooking.WebApi.Controllers
{

    [Route(ApiEndpoint.USER_API)]
    [ApiController]
    [InjectionFilter]
    public class UsersController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        private readonly IdentityService _service;
        [Inject]
        private readonly MemberService _memberService;
        [Inject]
        private readonly SystemService _sysService;

        #region OAuth
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(AuthorizationGrantModel model)
        {
            if (Settings.Instance.Mocking.Enabled)
            {
                switch (new Random().Next(1, 7))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return Ok(new
                        {
                            user_id = "mockuser",
                            access_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImUxNTIyNWE3LThhZmQtNDAxOC04NWJiLTRiNjBlYWQ2NDQwNiIsInJvbGUiOiJBZG1pbmlzdHJhdG9yIiwianRpIjoiOWM0YTg1OTctMWExZC00YTcwLWE5ZjMtOTYxNDU5MDhjOTE1Iiwic3ViIjoiZTE1MjI1YTctOGFmZC00MDE4LTg1YmItNGI2MGVhZDY0NDA2IiwibmJmIjoxNTkyNTc1MzUxLCJleHAiOjE1OTI2NjE3NTEsImlhdCI6MTU5MjU3NTM1MSwiaXNzIjoidGFyY2gxc3QiLCJhdWQiOiJ0YXJjaDFzdCJ9.jmYWsWCjyx8m_t2e5xa5cg4GSrh8m6pY9kXkEd0qR40",
                            token_type = "Bearer",
                            expires_utc = "2020-06-20T14:02:31Z",
                            issued_utc = "2020-06-19T14:02:31Z",
                            refresh_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImUxNTIyNWE3LThhZmQtNDAxOC04NWJiLTRiNjBlYWQ2NDQwNiIsIm5iZiI6MTU5MjU3NTM1MSwiZXhwIjoxNTkyNTc4OTUxLCJpYXQiOjE1OTI1NzUzNTEsImlzcyI6InJlZnJlc2hfdGFyY2gxc3QiLCJhdWQiOiJyZWZyZXNoX3RhcmNoMXN0In0.PVmWlEsEaYd4xv4t0pNDQh9H_Jp-ZdrLP9u904wlTqo",
                            email = "trungtnse130097@fpt.edu.vn",
                            roles = new[]
                            {
                                Settings.Instance.Mocking.LoginRole
                            }
                        });
                    case 5:
                        var vData = new ValidationData();
                        vData.Fail(code: AppResultCode.NotFound)
                            .Fail(mess: "Test", code: AppResultCode.FailValidation);
                        return BadRequest(AppResult.FailValidation(vData));
                    case 6:
                        throw new Exception("Test exception");
                }
            }
            var validationData = _service.ValidateLogin(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            AppUser entity;
            switch (model.grant_type)
            {
                case "password":
                case null:
                    {
                        entity = await
                            _service.AuthenticateAsync(model.username, model.password);
                        if (entity == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid username or password"));
                        }
                    }
                    break;
                case "refresh_token":
                    {
                        var validResult = _service.ValidateRefreshToken(model.refresh_token);
                        if (validResult == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid refresh token"));
                        }
                        entity = await _service.GetUserByIdAsync(validResult.Identity.Name);
                        if (entity == null)
                        {
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid user identity"));
                        }
                    }
                    break;
                case "firebase_token":
                    {
                        FirebaseToken validResult = await _service.ValidateFirebaseToken(model.firebase_token);
                        if (validResult == null)
                            return Unauthorized(AppResult
                                .Unauthorized(mess: "Invalid Firebase token"));
                        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(validResult.Uid);
                        bool checkEmailDomain = _service.ValidateEmailDomain(userRecord.Email);
                        if (!checkEmailDomain)
                            return Unauthorized(AppResult.InvalidEmailDomain());
                        entity = await _service.GetUserByUserNameAsync(userRecord.Uid);
                        if (entity == null)
                        {
                            entity = _service.ConvertToUser(userRecord);
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                var result = await _service
                                        .CreateUserWithoutPassAsync(entity);
                                if (!result.Succeeded)
                                {
                                    foreach (var err in result.Errors)
                                        ModelState.AddModelError(err.Code, err.Description);
                                    var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
                                    return BadRequest(builder);
                                }
                                _logger.CustomProperties(entity).Info("Register new user");
                                var memberEntity = _memberService.ConvertToMember(entity);
                                memberEntity = _memberService.CreateMember(memberEntity);
                                //log event
                                var ev = _sysService.GetEventForNewUser(
                                    $"{memberEntity.Email} has logged into system for the first time", UserId);
                                _sysService.CreateAppEvent(ev);
                                //end log event
                                context.SaveChanges();
                                transaction.Commit();
                            }
                        }
                    }
                    break;
                default:
                    return BadRequest(AppResult.Unsupported("Unsupported grant type"));
            }
            var identity =
                await _service.GetIdentityAsync(entity, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var utcNow = DateTime.UtcNow;
            var props = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
                ExpiresUtc = utcNow.AddHours(WebApi.Settings.Instance.TokenValidHours)
            };
            props.Parameters["refresh_expires"] = utcNow.AddHours(
                WebApi.Settings.Instance.RefreshTokenValidHours);
            var resp = _service.GenerateTokenResponse(principal, props, model.scope ?? AppOAuthScope.ROLES);
            _logger.CustomProperties(entity).Info("Login user");
            return Ok(resp);
        }
        #endregion

        [HttpGet("token-info")]
        [Authorize]
        public IActionResult GetTokenInfo()
        {
            var resp = new TokenInfo(User);
            return Ok(AppResult.Success(resp));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var validationData = _service.ValidateGetProfile(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = await _service.GetUserByIdAsync(UserId);
            var data = _service.GetUserProfile(entity);
            return Ok(AppResult.Success(data));
        }

#if DEBUG
        #region Administration
        [HttpPost("role")]
        public async Task<IActionResult> AddRole(AddRolesToUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var result = await _service.AddRolesForUserAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }

        [HttpDelete("role")]
        public async Task<IActionResult> RemoveRole(RemoveRolesFromUserModel model)
        {
            var entity = await _service.GetUserByUserNameAsync(model.username);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var result = await _service.RemoveUserFromRolesAsync(entity, model.roles);
            if (result.Succeeded)
                return NoContent();
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }
        #endregion
#endif
    }
}