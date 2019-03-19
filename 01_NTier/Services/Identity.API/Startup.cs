using Identity.API.Data;
using Identity.API.Managers;
using Identity.API.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.API
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment environment)
        {

            Configuration = configuration;
            Environment = environment;
            _loggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            services.AddScoped<IClaimsManager, ClaimsManager>();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc();
            services.AddSingleton<IProfileService, ProfileService>();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients(Configuration["CallbackUrl"]))
                .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
