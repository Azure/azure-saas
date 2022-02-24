using Dawn;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Saas.Catalog.Api.Models;

using System;

namespace Saas.Catalog.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">Application arguments</param>
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            ConfigureDatabase(host.Services);
            host.Run();
        }
        /// <summary>
        /// Configure a IHostBuilder
        /// </summary>
        /// <param name="args">Program args</param>
        /// <returns>Configured host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        ///  Call this to configure the database to create and initialize the database
        /// </summary>
        /// <param name="services">Service provider</param>
        private static void ConfigureDatabase(IServiceProvider services)
        {
            using IServiceScope scope = services.CreateScope();
            CatalogInitializer initializer = scope.ServiceProvider.GetRequiredService<CatalogInitializer>();

            initializer.CreateDatabase();
            initializer.SeedDatabase();
        }
    }
}
