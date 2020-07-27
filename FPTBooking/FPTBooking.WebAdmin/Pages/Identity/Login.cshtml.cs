using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using FPTBooking.Business.Models;
using FPTBooking.Business.Services;
using FPTBooking.Data.Helpers;
using FPTBooking.WebAdmin.Helpers;
using FPTBooking.WebAdmin.Models;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;
using FPTBooking.Business;
using FPTBooking.Data.Models;
using FirebaseAdmin.Auth;
using FPTBooking.Business.Helpers;
using FPTBooking.WebHelpers;
using FPTBooking.Business.Queries;
using FPTBooking.Data;

namespace FPTBooking.WebAdmin.Pages.Identity
{
    [InjectionFilter]
    public class LoginModel : BasePageModel<LoginModel>, IMessageModel
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        [Inject]
        protected readonly IdentityService identityService;
        [Inject]
        protected readonly MemberService memberService;
        [Inject]
        protected readonly SystemService systemService;
        [Inject]
        protected readonly DataContext context;
        public string Message { get; set; } = null;
        public string MessageTitle { get; set; } = null;
        public string Layout { get; set; } = null;

        public IActionResult OnGet(string return_url = Routing.DASHBOARD)
        {
            if (User.Identity.IsAuthenticated) return LocalRedirect(return_url);
            SetPageInfo();
            return Page();
        }

        public async Task<IActionResult> OnPost(Business.Models.LoginModel model,
            string return_url = Routing.DASHBOARD)
        {
            SetPageInfo();
            //this is auto handled by anti-forgery: return 400 bad request
            if (User.Identity.IsAuthenticated)
            {
                Message = "Can only logged in once at a time";
                MessageTitle = "Already logged in";
                return this.MessageView();
            }
            FirebaseToken validResult = await identityService.ValidateFirebaseToken(model.firebase_token);
            if (validResult == null)
            {
                Message = "Invalid firebase token";
                return Page();
            }
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(validResult.Uid);
            bool checkEmailDomain = identityService.ValidateEmailDomain(userRecord.Email);
            if (!checkEmailDomain)
            {
                Message = "Invalid email domain";
                return Page();
            }
            AppUser entity = await identityService.GetUserByEmailAsync(userRecord.Email);
            if (entity == null)
            {
                Message = "Access denied";
                return Page();
            }
            if (!entity.LoggedIn)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    Member memberEntity;
                    entity = identityService.UpdateUser(entity, userRecord);
                    var result = await identityService.UpdateUserAsync(entity);
                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                            ModelState.AddModelError(err.Code, err.Description);
                        var builder = ResultHelper.MakeInvalidAccountRegistrationResults(ModelState);
                        return BadRequest(builder);
                    }
                    memberEntity = memberService.Members.Id(entity.Id).FirstOrDefault();
                    memberEntity = memberService.UpdateMember(memberEntity, entity);
                    //log event
                    var ev = systemService.GetEventForNewUser(
                        $"{memberEntity.Email} has logged into system for the first time",
                        memberEntity.UserId);
                    systemService.CreateAppEvent(ev);
                    //end log event
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            #region Custom Signin for extra claims store
            var principal = await identityService.GetApplicationPrincipalAsync(entity);
            if (!principal.IsInRole(RoleName.ADMIN) && !Business.Settings.Instance.DevMode)
            {
                Message = "Access denied";
                return Page();
            }
            var utcNow = DateTime.UtcNow;
            var cookieProps = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
            };
            if (model.remember_me)
            {
                cookieProps.IsPersistent = true;
                cookieProps.ExpiresUtc = utcNow.AddHours(Settings.Instance.CookiePersistentHours);
            }
            await HttpContext.SignInAsync(principal.Identity.AuthenticationType,
                principal, cookieProps);
            #endregion
            _logger.CustomProperties(entity).Info("Login user");
            #region Generate token
            var identity =
                await identityService.GetIdentityAsync(entity, JwtBearerDefaults.AuthenticationScheme);
            principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
                ExpiresUtc = utcNow.AddHours(WebAdmin.Settings.Instance.TokenValidHours)
            };
            props.Parameters["refresh_expires"] = utcNow.AddHours(
                WebAdmin.Settings.Instance.RefreshTokenValidHours);
            var resp = identityService.GenerateTokenResponse(principal, props, AppOAuthScope.ROLES);
            _logger.CustomProperties(entity).Info("Login user");
            #endregion
            return LocalRedirect($"{Routing.INDEX}?access_token=" +
                        $"{resp.access_token}" +
                        $"&refresh_token={resp.refresh_token}" +
                        $"&expires_utc={resp.expires_utc}" +
                        $"&issued_utc={resp.issued_utc}" +
                        $"&token_type={resp.token_type}&" +
                        $"&return_url={return_url}");
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Log in"
            };
        }
    }
}
