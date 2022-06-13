using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Saas.SignupAdministration.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Saas.Application.Web;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Get Secrets From Azure Key Vault if in production. If not in production, secrets are automatically loaded in from the .NET secrets manager
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0

    // We don't want to fetch all the secrets for the other microservices in the app/solution, so we only fetch the ones with the prefix of "signupadmin-".
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix

    builder.Configuration.AddAzureKeyVault(
        new Uri(builder.Configuration[SR.KeyVaultProperty]),
        new DefaultAzureCredential(),
        new CustomPrefixKeyVaultSecretManager("signupadmin"));
}

builder.Services.AddRazorPages();

// Load the app settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(SR.AppSettingsProperty));

// Load the email settings 
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(SR.EmailOptionsProperty));

builder.Services.AddMvc();

// Add the workflow object
builder.Services.AddScoped<OnboardingWorkflowService, OnboardingWorkflowService>();

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
    .EnableTokenAcquisitionToCallDownstreamApi(
        builder.Configuration[SR.AdminServiceScopesProperty]
        .Split(" "))
    .AddSessionTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

// Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.AddOptions();
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection(SR.AzureAdB2CProperty));

// This is required for auth to work correctly when running in a docker container because of SSL Termination
// Remove this and the subsequent app.UseForwardedHeaders() line below if you choose to run the app without using containers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
app.UseForwardedHeaders();
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});
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

                endpoints.MapRazorPages();
});

AppHttpContext.Services = ((IApplicationBuilder)app).ApplicationServices;

app.Run();