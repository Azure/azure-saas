using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Saas.SignupAdministration.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Load the app settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(SR.AppSettingsProperty));

// Load the email settings 
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(SR.EmailOptionsProperty));

builder.Services.AddMvc();

// Add the workflow object
builder.Services.AddScoped<OnboardingWorkflow, OnboardingWorkflow>();

// Add this to allow for context to be shared outside of requests
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add the email object
builder.Services.AddScoped<IEmail, Email>();

// Required for the JsonPersistenceProvider
// Should be replaced based on the persistence scheme
builder.Services.AddDistributedMemoryCache();

// TODO: Replace with your implementation of persistence provider
// Session persistence is the default
builder.Services.AddScoped<IPersistenceProvider, JsonSessionPersistenceProvider>();

// Add the user details that come back from B2C
builder.Services.AddScoped<IApplicationUser, ApplicationUser>();

builder.Services.AddHttpClient<IAdminServiceClient, AdminServiceClient>()
    .ConfigureHttpClient(client =>
   client.BaseAddress = new Uri(builder.Configuration[SR.AdminServiceBaseUrl]));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration[SR.AppInsightsConnectionProperty]);

// builder.Configuration to sign-in users with Azure AD B2C
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C)
    .EnableTokenAcquisitionToCallDownstreamApi(builder.Configuration["AppSettings:AdminServiceScopes"].Split(" "))
    .AddSessionTokenCaches();
    //.AddInMemoryTokenCaches();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

// Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.AddOptions();
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
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

AppHttpContext.Services = ((IApplicationBuilder)app).ApplicationServices;

app.Run();