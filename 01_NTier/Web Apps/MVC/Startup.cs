using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Basket.API.Repositories;
using Basket.API.Services;
using Catalog.API.Data;
using Catalog.API.Queries;
using Catalog.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Models.ViewModels;
using MVC.AutoMapper;
using MVC.Model.UserData;
using Newtonsoft.Json.Serialization;
using Ordering.Repositories;
using Ordering.Services;
using Serilog;
using Services;

namespace MVC
{
    public class Startup
    {
        private readonly CatalogStartup catalogStartup;
        private readonly BasketStartup basketStartup;
        private readonly OrderingStartup orderingStartup;

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;

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
                options.UseSqlServer(Configuration["CatalogConnectionString"])
            );

            services.AddScoped<DbContext, ApplicationDbContext>();
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetService<ApplicationDbContext>();
            services.AddSingleton(context);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }

    public class BasketStartup
    {
        public BasketStartup(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddTransient<IBasketRepository, BasketRepository>();
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
            var context = serviceProvider.GetService<Ordering.API.ApplicationContext>();
            services.AddSingleton<Ordering.API.ApplicationContext>(context);

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddTransient<IOrderingService, OrderingService>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }

}
