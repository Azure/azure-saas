using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saas.LandingSignup.Web.Data;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Repositories;
using Saas.LandingSignup.Web.Services;
using System;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString(SR.IdentityDbConnectionProperty)));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddControllersWithViews();
            services.AddScoped<TenantRepository, TenantRepository>();
            services.AddScoped<CustomerRepository, CustomerRepository>();

            var appSettings = Configuration.GetSection(SR.AppSettingsProperty);

            services.Configure<AppSettings>(appSettings);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1);
            });
            
            services.AddApplicationInsightsTelemetry(Configuration[SR.AppInsightsConnectionProperty]);
            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection(SR.CosmosDbProperty)).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(SR.ErrorRoute);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                var routes = endpoints.MapControllerRoute(name: SR.DefaultName, pattern: SR.MapControllerRoutePattern);

                if (env.IsDevelopment())
                {
                    routes.WithMetadata(new AllowAnonymousAttribute());
                }

                endpoints.MapRazorPages();
            });
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key. 
        /// </summary>
        /// <returns></returns>
        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection.GetSection(SR.DatabaseNameProperty).Value;
            var containerName = configurationSection.GetSection(SR.ContainerNameProperty).Value;
            var account = configurationSection.GetSection(SR.AccountProperty).Value;
            var key = configurationSection.GetSection(SR.KeyProperty).Value;
           
            var client = new CosmosClient(account, key);
            var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            
            await database.Database.CreateContainerIfNotExistsAsync(containerName, SR.CosmosNamePartitionKey);

            return cosmosDbService;
        }
    }
}
