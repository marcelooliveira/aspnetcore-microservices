using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Basket.API;
using Basket.API.Model;
using Basket.API.Services;
using Catalog.API.Data;
using Catalog.API.Queries;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.ViewModels;
using MVC.AutoMapper;
using MVC.Model.Redis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Ordering.Repositories;
using Ordering.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Services;
using Services.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CatalogStartup catalogStartup;
        private readonly BasketStartup basketStartup;
        private readonly OrderingStartup orderingStartup;

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;

            var configurationByFile = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationByFile)
                .CreateLogger();

            catalogStartup = 
                new CatalogStartup(configuration);

            basketStartup =
                new BasketStartup(configuration, loggerFactory);

            orderingStartup =
                new OrderingStartup(configuration);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CatalogProfile());
                mc.AddProfile(new OrderingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(typeof(IMapper), mapper);

            services.AddHttpContextAccessor();
            services.AddTransient<IUserRedisRepository, UserRedisRepository>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<IBasketService, BasketService>();
            services.AddTransient<ISessionHelper, SessionHelper>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


            services.AddMvc()
                .AddJsonOptions(a => a.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddDistributedMemoryCache();
            services.AddSession();
            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(Configuration["RedisConnectionString"], true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            catalogStartup.ConfigureServices(services);
            basketStartup.ConfigureServices(services);
            orderingStartup.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{code?}");
            });

            catalogStartup.Configure(app, env, loggerFactory);
            basketStartup.Configure(app, env, loggerFactory);
            orderingStartup.Configure(app, env, loggerFactory);
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }

    public class CatalogStartup
    {
        public CatalogStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProductQueries, ProductQueries>();
            services.AddTransient<ICatalogAPIService, CatalogAPIService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SQLitePCL.Batteries_V2.Init();

        }
    }

    public class BasketStartup
    {
        private readonly ILoggerFactory _loggerFactory;

        public BasketStartup(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<BasketConfig>>().Value;
                var configuration = ConfigurationOptions.Parse(Configuration["RedisConnectionString"], true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddTransient<IBasketRepository, RedisBasketRepository>();
            services.AddTransient<IBasketAPIService, BasketAPIService>();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSession();
            app.UseAuthentication();
        }
    }

    public class OrderingStartup
    {
        private readonly ILoggerFactory _loggerFactory;

        public OrderingStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration["OrderingConnectionString"];

            services.AddDbContext<Ordering.API.ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddScoped<DbContext, Ordering.API.ApplicationContext>();
            var serviceProvider = services.BuildServiceProvider();
            var contexto = serviceProvider.GetService<Ordering.API.ApplicationContext>();
            services.AddSingleton<Ordering.API.ApplicationContext>(contexto);

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddTransient<IOrderingService, OrderingService>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }

}
