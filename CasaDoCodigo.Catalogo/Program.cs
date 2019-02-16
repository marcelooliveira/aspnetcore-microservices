using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Catalog.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Catalog.API";

            IWebHost host = BuildWebHost(args);
            await SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost
                    .CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .ConfigureLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.AddSerilog();
                    })
                    .Build();
        }
    }
}
