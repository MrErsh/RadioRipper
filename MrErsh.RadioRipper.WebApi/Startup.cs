using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MrErsh.RadioRipper.Dal;
using MrErsh.RadioRipper.IdentityDal;
using MrErsh.RadioRipper.WebApi.Auth;
using MrErsh.RadioRipper.WebApi.Bl;
using MrErsh.RadioRipper.WebApi.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Config = MrErsh.RadioRipper.WebApi.Configuration;

namespace MrErsh.RadioRipper.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        #region Public methods

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAppSettings(services, Configuration);
            ConfigureDatabaseServices(services);
            ConfigureSecurityServices(services, Configuration);
            ConfigureSwagger(services);

            services.AddSingleton<IRipperManager, RipperManager>();
            services.AddSingleton<IRipperFactory, RipperFactory>();
            services.Configure<Config.Ripper>(Configuration.GetSection(Config.Ripper.SECTION_NAME));

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            UpdateDatabase(app);

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "api_v1"); });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            services.GetService<IRipperManager>().RestartAllAsync();
        }

        #endregion

        #region Private methods

        #region Configure services

        private static void ConfigureAppSettings(IServiceCollection services, IConfiguration config)
        {
            services.Configure<SecurityJwtOptions>(config.GetSection(SecurityJwtOptions.SECTION));
        }

        private void ConfigureDatabaseServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityDbContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));
            services.AddDbContextFactory<RadioDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        }

        private static void ConfigureSecurityServices(IServiceCollection services, IConfiguration config)
        {
            Func<RedirectContext<CookieAuthenticationOptions>, Task> CreateEventToErrorHandler(int statusCode) =>
                (context) =>
                {
                    if (/*context.Request.Path.StartsWithSegments("/api") && */context.Response.StatusCode == StatusCodes.Status200OK)
                        context.Response.StatusCode = statusCode;
                    return Task.CompletedTask;
                };

            services.ConfigureApplicationCookie(opt =>
            {
                opt.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = CreateEventToErrorHandler(StatusCodes.Status401Unauthorized),
                    OnRedirectToAccessDenied = CreateEventToErrorHandler(StatusCodes.Status403Forbidden)
                };
            });

            var jwtOptions = config.GetSection<SecurityJwtOptions>(SecurityJwtOptions.SECTION);
            var signer = new JwtEncodingDescription(jwtOptions.SigningKey);

            services.AddSingleton<IJwtEncodingDescription>(signer)
                    .AddAuthentication(opt =>
                    {
                        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = signer.Key,

                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromSeconds(5)
                        };
                    });

            services.AddIdentity<User, IdentityRole>(
                opt =>
                {
                    opt.User.RequireUniqueEmail = false;
                    opt.Password.RequiredLength = 5;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireDigit = false;
                })
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.AddScoped<UserManager<User>, AspNetUserManager<User>>();
            services.AddTransient<RoleManager<IdentityRole>, AspNetRoleManager<IdentityRole>>();
            services.AddScoped<IAuthorizationHandler, AuthorizePermissionHandler>();
            services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, IdentityRole>>();
            services.AddAuthorization(CreatePolicies);
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RadioRipper API",
                    Version = "v1"
                });

                //диалог ввода JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        #endregion

        #region Configure

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using var identityContext = scope.ServiceProvider.GetService<IdentityDbContext>();
                if (identityContext.Database.GetPendingMigrations().Any())
                    identityContext.Database.Migrate();

                using var appContext = app.ApplicationServices
                    .GetRequiredService<IDbContextFactory<RadioDbContext>>()
                    .CreateDbContext();

                var isNewDb = !appContext.Database.GetAppliedMigrations().Any();
                var pending = appContext.Database.GetPendingMigrations();
                if (appContext.Database.GetPendingMigrations().Any())
                    appContext.Database.Migrate();

                if (isNewDb)
                {
                    var initializator = new DemoUserDbInitializer();
                    initializator.AddDefaultStations(appContext);
#if DEBUG
                    initializator.AddDefaultStations(appContext, SeedData.AdminUserId);
#endif
                    appContext.SaveChanges();
                }
            }
        }

        private static void CreatePolicies(AuthorizationOptions options)
        {
            foreach (var perm in PermissionClaimsProvider.AllClaims)
            {
                options.AddPolicy(perm.Value, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(perm.Value));
                });
            }
        }

        #endregion
        #endregion
    }
}
