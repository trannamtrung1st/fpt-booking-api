using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FPTBooking.Business;
using FPTBooking.Data;
using FPTBooking.Data.Models;
using TNT.Core.Helpers.DI;
using System.Security.Claims;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FPTBooking.Business.Helpers;

namespace FPTBooking.WebApi
{
    public class Startup
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Ijg1NzI0ODIyLTI2ZWQtNGIyNC05YTNmLTAxNGI4NTlkNjFiZCIsInJvbGUiOiJVc2VyIiwidXNlcm5hbWUiOiIzRVpVQ09iNUw0VXB6ckJDaFpTUGRDTVFTd0gzIiwicGhvdG9fdXJsIjoiaHR0cHM6Ly9saDYuZ29vZ2xldXNlcmNvbnRlbnQuY29tLy1MaEJfelR0WkpmNC9BQUFBQUFBQUFBSS9BQUFBQUFBQUFBQS9BTVp1dWNrY0kzWE1WbUJZenVkeVdYMFpkaWZMclBBcHR3L3M5Ni1jL3Bob3RvLmpwZyIsImVtYWlsIjoidHJ1bmd0bnNlMTMwMDk3QGZwdC5lZHUudm4iLCJqdGkiOiJlNTgwN2VlYy0xYjRmLTQ1MDUtYWQyMy02ZjU3MmM0MmJiYzciLCJzdWIiOiI4NTcyNDgyMi0yNmVkLTRiMjQtOWEzZi0wMTRiODU5ZDYxYmQiLCJuYmYiOjE1OTQyMDMzNzgsImV4cCI6MTg3NDIwMDU3OCwiaWF0IjoxNTk0MjAzMzc4LCJpc3MiOiJmcHRib29raW5nMXN0IiwiYXVkIjoiZnB0Ym9va2luZzFzdCJ9.XvPad_mQC1PPXSsYGOOTfr6T-8Gy0E-qyonLrN4zOOQ
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configuration.Bind("BusinessSettings", Business.Settings.Instance);
            configuration.Bind("WebApiSettings", WebApi.Settings.Instance);
        }

        public IConfiguration Configuration { get; }
        public static string WebRootPath { get; private set; }
        public static string MapPath(string path, string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = WebRootPath;
            }

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(basePath, path);
        }

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
            //connStr = connStr.Replace("{envConfig}", ".Test");
            connStr = connStr.Replace("{envConfig}", "");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connStr).UseLazyLoadingProxies());
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            Data.Global.Init(services);
            Business.Global.Init(services);
            #region OAuth
            services.AddIdentityCore<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            }).AddRoles<AppRole>()
            .AddDefaultTokenProviders()
            .AddSignInManager()
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            services.AddSingleton(new DefaultDateTimeModelBinder());
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new QueryObjectModelBinderProvider());
            }).AddNewtonsoftJson();
            services.AddSwaggerGenNewtonsoftSupport();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                var requirement = new OpenApiSecurityRequirement();
                requirement[new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                }] = new string[] { };
                c.AddSecurityRequirement(requirement);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            PrepareEnvironment(env);
            WebRootPath = env.WebRootPath;
            Settings.Instance.WebRootPath = env.WebRootPath;
            app.UseExceptionHandler($"/{ApiEndpoint.ERROR}");
            app.UseStaticFiles();
            app.UseRouting();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                //builder.AllowAnyOrigin();
                builder.SetIsOriginAllowed(origin =>
                {
                    return true;
                });
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void PrepareEnvironment(IWebHostEnvironment env)
        {
        }
    }
}