using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Basket.API;
using Basket.API.Model;
using Basket.API.Services;
using Catalog.API.Data;
using Catalog.API.Queries;
using Catalog.API.Services;
using HealthChecks.UI.Client;
using MediatR;
using Messages.EventHandling;
using Messages.Events;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.ViewModels;
using MVC.AutoMapper;
using MVC.Commands;
using MVC.Model.Redis;
using MVC.SignalR;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Ordering.API.SignalR;
using Ordering.Commands;
using Ordering.Repositories;
using Polly;
using Polly.Extensions.Http;
using Rebus.Config;
using Rebus.ServiceProvider;
using Serilog;
using Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
            var uri = new Uri(Configuration["ApiUrl"]);
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = uri
            };

            services.AddAutoMapper();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CatalogProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(typeof(IMapper), mapper);

            services.AddSingleton(typeof(HttpClient), httpClient);
            services.AddHttpContextAccessor();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<IBasketService, BasketService>();
            services.AddTransient<ISessionHelper, SessionHelper>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRedis(
                    Configuration["RedisConnectionString"],
                    name: "redis-check",
                    tags: new string[] { "redis" });

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
            services.AddAuthorization();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = Configuration["IdentityUrl"];
                    options.BackchannelHttpHandler = new HttpClientHandler() { Proxy = new WebProxy() };
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "MVC";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnUserInformationReceived = (context) =>
                        {
                            if (!(context.Principal.Identity is ClaimsIdentity claimsId))
                            {
                                throw new Exception();
                            }

                            // Get a list of all claims attached to the UserInformationRecieved context
                            var ctxClaims = context.User.Children().ToList();

                            foreach (var ctxClaim in ctxClaims)
                            {
                                var claimType = ctxClaim.Path;
                                var token = ctxClaim.FirstOrDefault();
                                if (token == null)
                                {
                                    continue;
                                }

                                var claims = new List<Claim>();
                                if (token.Children().Any())
                                {
                                    claims.AddRange(
                                        token.Children()
                                            .Select(c => new Claim(claimType, c.Value<string>())));
                                }
                                else
                                {
                                    claims.Add(new Claim(claimType, token.Value<string>()));
                                }

                                foreach (var claim in claims)
                                {
                                    if (!claimsId.Claims.Any(
                                        c => c.Type == claim.Type &&
                                             c.Value == claim.Value))
                                    {
                                        claimsId.AddClaim(claim);
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.Scope.Add("Basket.API");
                    options.Scope.Add("Ordering.API");
                    options.Scope.Add("offline_access");
                });
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<IUserRedisRepository, UserRedisRepository>();
            services.AddMediatR(typeof(UserNotificationCommand).GetTypeInfo().Assembly);

            //services.AddHttpClient<IBasketService, BasketService>()
            //       .AddPolicyHandler(GetRetryPolicy())
            //       .AddPolicyHandler(GetCircuitBreakerPolicy());

            //services.AddHttpClient<ICatalogService, CatalogService>()
            //       .AddPolicyHandler(GetRetryPolicy())
            //       .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IOrderService, OrderService>()
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy());

            catalogStartup.ConfigureServices(services);
            basketStartup.ConfigureServices(services);
            orderingStartup.ConfigureServices(services);

            RegisterRebus(services);
        }

        private void RegisterRebus(IServiceCollection services)
        {
            services.AutoRegisterHandlersFromAssemblyOf<CheckoutEventHandler>();

            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(Configuration["RabbitMQConnectionString"], Configuration["RabbitMQInputQueueName"])))
                .AddTransient<DbContext, Ordering.API.ApplicationContext>()
                .AutoRegisterHandlersFromAssemblyOf<CheckoutEvent>();
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

            app.UseAuthentication();

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{code?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<UserCounterDataHub>("/usercounterdatahub",
                    options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                    });
            });

            catalogStartup.Configure(app, env, loggerFactory);
            basketStartup.Configure(app, env, loggerFactory);
            orderingStartup.Configure(app, env, loggerFactory);


            app.UseRebus(
                async (bus) =>
                {
                    await bus.Subscribe<CheckoutEvent>();
                });
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
            services.AddTransient<IIdentityService, IdentityService>();
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
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            string connectionString = Configuration["OrderingConnectionString"];

            services.AddDbContext<Ordering.API.ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddScoped<DbContext, Ordering.API.ApplicationContext>();
            var serviceProvider = services.BuildServiceProvider();
            var contexto = serviceProvider.GetService<Ordering.API.ApplicationContext>();
            services.AddSingleton<Ordering.API.ApplicationContext>(contexto);

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IMediator, NoMediator>();
            services.AddScoped<IRequest<bool>, CreateOrderCommand>();
            services.AddMediatR(typeof(CreateOrderCommand).GetTypeInfo().Assembly);
            //RegisterRebus(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }

}
