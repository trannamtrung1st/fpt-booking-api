using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FPTBooking.Business;
using FPTBooking.Business.Services;
using FPTBooking.Data.Models;
using FPTBooking.WebAdmin.Pages.Shared;
using TNT.Core.Helpers.DI;
using System.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FPTBooking.Business.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FPTBooking.Data;

namespace FPTBooking.WebAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configuration.Bind("BusinessSettings", Business.Settings.Instance);
            configuration.Bind("WebAdminSettings", WebAdmin.Settings.Instance);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Settings.Instance.FirebaseCredentials),
            });
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
            services.AddServiceInjection();
            var connStr = Configuration.GetConnectionString("DataContext");
#if TEST
            connStr = connStr.Replace("{envConfig}", ".Test");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connStr).UseLazyLoadingProxies());
            Data.Global.Init(services);
            var fapSecret = File.ReadAllText(Settings.Instance.FapSecretFile);
            Business.Global.Init(services, fapSecret);
            #region OAuth
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<DataContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            //required
            services.AddAuthentication()
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = JWT.ISSUER,
                        ValidAudience = JWT.AUDIENCE,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(JWT.SECRET_KEY)),
                        ClockSkew = TimeSpan.Zero
                    };
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            if (Settings.Instance.Mocking.Enabled)
                            {
                                var identity = new ClaimsIdentity("Mocking");
                                identity.AddClaim(new Claim(ClaimTypes.Name, "mockuser"));
                                identity.AddClaims(new List<Claim>
                                {
                                    new Claim(ClaimTypes.Role, RoleName.ADMIN),
                                    new Claim(ClaimTypes.Role, RoleName.MANAGER),
                                    new Claim(ClaimTypes.Role, RoleName.ROOM_CHECKER),
                                    new Claim(ClaimTypes.Role, RoleName.USER),
                                });
                                context.Principal = new ClaimsPrincipal(identity);
                                context.Success();
                            }
                            return Task.CompletedTask;
                            //StringValues values;
                            //if (!context.Request.Query.TryGetValue("access_token", out values))
                            //    return Task.CompletedTask;
                            //var token = values.FirstOrDefault();
                            //context.Token = token;
                            //return Task.CompletedTask;
                        }
                    };
                });
            #endregion
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = Routing.ACCESS_DENIED;
                options.ExpireTimeSpan = TimeSpan.FromHours(
                    WebAdmin.Settings.Instance.CookiePersistentHours);
                options.LoginPath = Routing.LOGIN;
                options.LogoutPath = Routing.LOGOUT;
                options.ReturnUrlParameter = "return_url";
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = async (c) =>
                {
                    var identity = c.Principal.Identity as ClaimsIdentity;
                    //extra claims will be expired after amount of time
                    if (identity.FindFirst(AppClaimType.UserName)?.Value == null)
                    {
                        var identityService = c.HttpContext.RequestServices.GetRequiredService<IdentityService>();
                        var entity = await identityService.GetUserByUserNameAsync(identity.Name);
                        var principal = await identityService.GetApplicationPrincipalAsync(entity);
                        c.ReplacePrincipal(principal);
                        c.ShouldRenew = true;
                    }
                    await SecurityStampValidator.ValidatePrincipalAsync(c);
                };
            });
            services.AddScoped<Layout>();

            services.AddSingleton(new DefaultDateTimeModelBinder());
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new QueryObjectModelBinderProvider());
            }).AddNewtonsoftJson();
            services.AddRazorPages(options =>
            {
                var allowAnnonymousPages = new[] {
                    "/AccessDenied", "/Error", "/Status", "/Identity/Login", "/Identity/Register" };
                var authorizeFolders = new[] { "/" };
                foreach (var f in authorizeFolders)
                    options.Conventions.AuthorizeFolder(f);
                foreach (var p in allowAnnonymousPages)
                    options.Conventions.AllowAnonymousToPage(p);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(Routing.ERROR);
            app.UseStatusCodePagesWithReExecute(Routing.STATUS, "?code={0}");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
