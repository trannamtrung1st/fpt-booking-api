﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data;
using FPTBooking.Data.Models;
using TNT.Core.Helpers.DI;
using FirebaseAdmin.Auth;

namespace FPTBooking.Business.Services
{

    public class IdentityService : Service
    {
        [Inject]
        private readonly UserManager<AppUser> _userManager;
        [Inject]
        private readonly SignInManager<AppUser> _signInManager;
        [Inject]
        private readonly RoleManager<AppRole> _roleManager;
        [Inject]
        private readonly MemberService _memberService;

        public IdentityService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query AppUser
        public IQueryable<AppUser> Users
        {
            get
            {
                return _userManager.Users;
            }
        }
        #endregion

        #region Role
        public IQueryable<AppRole> Roles
        {
            get
            {
                return _roleManager.Roles;
            }
        }

        public AppRole GetRoleByName(string name)
        {
            return Roles.FirstOrDefault(r => r.Name == name);
        }

        public async Task<IdentityResult> RemoveRoleAsync(AppRole entity)
        {
            return await _roleManager.DeleteAsync(entity);
        }

        protected void PrepareCreate(AppRole entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            var result = await _roleManager.CreateAsync(entity);
            return result;
        }

        public async Task<IdentityResult> UpdateRoleAsync(AppRole entity,
            UpdateRoleModel model)
        {
            model.CopyTo(entity);
            var result = await _roleManager.UpdateAsync(entity);
            return result;
        }

        public ValidationData ValidateGetProfile(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetRoles(
            ClaimsPrincipal principal)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateRole(
            ClaimsPrincipal principal, CreateRoleModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateRole(
            ClaimsPrincipal principal, UpdateRoleModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteRole(
            ClaimsPrincipal principal, AppRole entity)
        {
            return new ValidationData();
        }
        #endregion

        #region User
        public AppUser ConvertToUser(UserRecord firebaseUser)
        {
            var entity = new AppUser
            {
                UserName = firebaseUser.Uid,
                FullName = firebaseUser.DisplayName,
                Email = firebaseUser.Email,
                EmailConfirmed = firebaseUser.EmailVerified,
                PhoneNumber = firebaseUser.PhoneNumber,
                PhotoUrl = firebaseUser.PhotoUrl
            };
            return entity;
        }

        public AppUser ConvertToUser(RegisterModel model)
        {
            var entity = new AppUser { UserName = model.username, FullName = model.full_name };
            return entity;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> CreateUserWithoutPassAsync(AppUser entity)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity);
            if (!result.Succeeded) return result;
            result = await _userManager.AddToRoleAsync(entity, RoleName.USER);
            return result;
        }

        public async Task<SignInResult> PasswordSignInAsync(
            string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(userName: username,
                   password,
                   isPersistent, lockoutOnFailure);
            return result;
        }

        public async Task SignInAsync(AppUser user, AuthenticationProperties props)
        {
            await _signInManager.SignInAsync(user: user, authenticationProperties: props);
        }

        public List<Claim> GetExtraClaims(AppUser entity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(AppClaimType.UserName, entity.UserName));
            claims.Add(new Claim(AppClaimType.PhotoUrl, entity.PhotoUrl));
            claims.Add(new Claim(ClaimTypes.Email, entity.Email));
            return claims;
        }

        public async Task SignInWithExtraClaimsAsync(AppUser entity, bool isPersistent)
        {
            var extraClaims = GetExtraClaims(entity);
            //SignInWithClaimsAsync: for additional claims
            await _signInManager.SignInWithClaimsAsync(user: entity,
                isPersistent: isPersistent, extraClaims);
        }

        protected void PrepareCreate(AppUser entity)
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public object GetUserProfile(AppUser entity)
        {
            return new
            {
                full_name = entity.FullName,
                email = entity.Email,
                id = entity.Id,
                phone = entity.PhoneNumber
            };
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser entity)
        {
            return await _userManager.UpdateAsync(entity);
        }

        public async Task<IdentityResult> AddRolesForUserAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> RemoveUserFromRolesAsync(AppUser entity, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(entity, roles);
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser entity, string password)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            return result;
        }

        public async Task<IdentityResult> CreateUserWithRolesTransactionAsync(AppUser entity, string password,
            IEnumerable<string> roles = null)
        {
            PrepareCreate(entity);
            var result = await _userManager.CreateAsync(entity, password);
            if (!result.Succeeded)
                return result;
            if (roles != null)
                result = await _userManager.AddToRolesAsync(entity, roles);
            return result;
        }

        public ValidationData ValidateLogin(
            ClaimsPrincipal principal, AuthorizationGrantModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateRegister(
            ClaimsPrincipal principal, RegisterModel model)
        {
            return new ValidationData();
        }
        #endregion

        #region OAuth
        public TokenResponseModel GenerateTokenResponse(ClaimsPrincipal principal,
            AuthenticationProperties properties, string scope = null)
        {
            #region Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.Default.GetBytes(JWT.SECRET_KEY);
            var issuer = JWT.ISSUER;
            var audience = JWT.AUDIENCE;
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, principal.Identity.Name));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = properties.ExpiresUtc?.UtcDateTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            #endregion
            var resp = new TokenResponseModel();
            resp.user_id = identity.Name;
            resp.is_view_only_user = _memberService.Members.IsViewOnlyUser().Id(identity.Name).Any();
            resp.email = identity.FindFirst(ClaimTypes.Email)?.Value;
            resp.photo_url = identity.FindFirst(AppClaimType.PhotoUrl)?.Value;
            resp.access_token = tokenString;
            resp.token_type = "bearer";
            if (properties.ExpiresUtc != null)
                resp.expires_utc = properties.ExpiresUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (properties.IssuedUtc != null)
                resp.issued_utc = properties.IssuedUtc?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            #region Handle scope
            if (scope != null)
            {
                var scopes = scope.Split(' ');
                foreach (var s in scopes)
                {
                    switch (s)
                    {
                        case AppOAuthScope.ROLES:
                            resp.roles = identity.FindAll(identity.RoleClaimType)
                                .Select(c => c.Value).ToList();
                            break;
                    }
                }
            }
            #endregion
            #region Refresh Token
            key = Encoding.Default.GetBytes(JWT.REFRESH_SECRET_KEY);
            issuer = JWT.REFRESH_ISSUER;
            audience = JWT.REFRESH_AUDIENCE;
            var id = identity.Name;
            identity = new ClaimsIdentity(
                identity.Claims.Where(c => c.Type == identity.NameClaimType),
                identity.AuthenticationType);

            var refresh_expires = (properties.Parameters["refresh_expires"]
                as DateTimeOffset?)?.UtcDateTime;
            tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = properties.IssuedUtc?.UtcDateTime,
                Expires = refresh_expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                NotBefore = properties.IssuedUtc?.UtcDateTime
            };

            token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);
            resp.refresh_token = tokenString;
            #endregion
            return resp;
        }

        public ClaimsPrincipal ValidateRefreshToken(string tokenStr)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken secToken;
                var param = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JWT.REFRESH_ISSUER,
                    ValidAudience = JWT.REFRESH_AUDIENCE,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(JWT.REFRESH_SECRET_KEY)),
                    ClockSkew = TimeSpan.Zero
                };
                return tokenHandler.ValidateToken(tokenStr, param, out secToken);
            }
            catch (Exception) { }
            return null;
        }

        public async Task<AppUser> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
                return null;
            return user;
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(AppUser entity, string scheme)
        {
            var identity = new ClaimsIdentity(scheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = await _userManager.GetClaimsAsync(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            claims = GetExtraClaims(entity);
            identity.AddClaims(claims);
            return identity;
        }

        //for IdentityCookie
        public async Task<ClaimsPrincipal> GetApplicationPrincipalAsync(AppUser entity)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(entity);
            var identity = principal.Identity as ClaimsIdentity;
            identity.AddClaim(new Claim(ClaimTypes.Name, entity.Id));
            var claims = GetExtraClaims(entity);
            var roles = await _userManager.GetRolesAsync(entity);
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));
            identity.AddClaims(claims);
            return principal;
        }
        #endregion

        #region External Login
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            return _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        }

        public async Task<AppUser> AuthenticateExternalAsync(string provider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            return user;
        }

        public async Task<IdentityResult> AddLoginToUserAsync(AppUser entity, ExternalLoginInfo info)
        {
            var result = await _userManager.AddLoginAsync(entity, info);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{entity.Id}'.");
            return result;
        }
        #endregion

        #region Validation
        public async Task<FirebaseToken> ValidateFirebaseToken(string tokenStr)
        {
            FirebaseToken decodedToken = null;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenStr);
            }
            catch (FirebaseAuthException)
            {
            }
            return decodedToken;
        }

        public bool ValidateEmailDomain(string email)
        {
            if (Settings.Instance.DevMode) return true;
            string[] emails = email.Trim().Split("@");
            if (emails.Length == 2)
                return emails[1] == AllowedEmailDomains.FPT_DOMAIN;
            return false;
        }
        #endregion

    }
}