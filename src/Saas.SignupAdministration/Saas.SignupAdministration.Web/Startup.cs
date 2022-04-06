using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;

namespace Saas.SignupAdministration.Web
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
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString(SR.IdentityDbConnectionProperty)));
            services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    // Default SignIn settings.
            //    options.SignIn.RequireConfirmedEmail = false;
            //    options.SignIn.RequireConfirmedPhoneNumber = false;

            //    // Default Password settings.
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 0;
            //});

            services.AddRazorPages();

            services.Configure<AppSettings>(Configuration.GetSection(SR.AppSettingsProperty));

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddScoped<OnboardingWorkflow, OnboardingWorkflow>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // TODO: Replace with your implementation of persistence provider
            // Session persistence is the default
            services.AddScoped<IPersistenceProvider, JsonSessionPersistenceProvider>();

            services.AddHttpClient<IAdminServiceClient, AdminServiceClient>()
                .ConfigureHttpClient(client =>
               client.BaseAddress = new Uri(Configuration[SR.AdminServiceBaseUrl]));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            services.AddApplicationInsightsTelemetry(Configuration[SR.AppInsightsConnectionProperty]);

            services.AddDbContext<SaasSignupAdministrationWebContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SaasSignupAdministrationWebContext")));

            // Configuration to sign-in users with Azure AD B2C
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, Constants.AzureAdB2C);
            services.AddControllersWithViews().AddMicrosoftIdentityUI();
            // Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("AzureAdB2C"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
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
                // admin
                endpoints.MapControllerRoute(
                    name: "Admin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                
                // default
                endpoints.MapControllerRoute(name: SR.DefaultName, pattern: SR.MapControllerRoutePattern);
                //if (env.IsDevelopment())
                //{
                //    routes.WithMetadata(new AllowAnonymousAttribute());

                //}

                endpoints.MapRazorPages();
            });

            AppHttpContext.Services = app.ApplicationServices;
        }
    }
}
