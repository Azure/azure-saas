using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Saas.Catalog.Api.Models;

using System;

namespace Saas.Catalog.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            ConfigureDatabase(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void ConfigureDatabase(IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();

            IServiceProvider services = scope.ServiceProvider;
            
            ILoggerFactory loggerFactory = services.GetService<ILoggerFactory>();
            ILogger logger = loggerFactory?.CreateLogger<Program>();
            
            try
            {
                var context = services.GetRequiredService<CatalogDbContext>();
                context.Initialize(logger);
            }
            catch(Exception ex)
            {
                logger?.LogCritical(ex, "An error occured while creating the Catalog database.");
                throw;
            }
        }
    }
}
