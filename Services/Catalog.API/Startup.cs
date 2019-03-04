using Catalog.API.Data;
using Catalog.API.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Create Serilog Elasticsearch logger
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["ELASTICSEARCH_URL"]))
               {
                   MinimumLogEventLevel = LogEventLevel.Information,
                   AutoRegisterTemplate = true
               })
               .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProductQueries, ProductQueries>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "The Grocery Store - Catalog",
                    Description = "Product Catalog API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Marcelo Oliveira",
                        Email = "mclricardo@gmail.com",
                        Url = "https://twitter.com/twmoliveira"
                    },
                    License = new License
                    {
                        Name = "Licença XPTO 4567",
                        Url = "https://example.com/license"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureSwaggerGen(options =>
            {
                // UseFullTypeNameInSchemaIds replacement for .NET Core
                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
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
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Grocery Store - Catalog");
            });

            SQLitePCL.Batteries_V2.Init();
            app.UseHttpsRedirection();

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
            app.UseMvc();
        }
    }
}
