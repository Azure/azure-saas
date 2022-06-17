using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Saas.Application.Web;
using Saas.Application.Web.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;

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
        // TODO (SaaS): Update secret manager key to one specific to the application
        new CustomPrefixKeyVaultSecretManager("saasapplication"));
}

builder.Services.AddRazorPages();
// Load the app settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(SR.AppSettingsProperty));

builder.Services.AddMvc();
// Add this to allow for context to be shared outside of requests
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Required for the JsonPersistenceProvider
// Should be replaced based on the persistence scheme
builder.Services.AddDistributedMemoryCache();

// TODO (SaaS): Replace with your implementation of persistence provider
// Session persistence is the default
builder.Services.AddScoped<IPersistenceProvider, JsonSessionPersistenceProvider>();

// Add the user details that come back from B2C
builder.Services.AddScoped<IApplicationUser, ApplicationUser>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddHttpClient<IAdminServiceClient, AdminServiceClient>()
    .ConfigureHttpClient(client =>
   client.BaseAddress = new Uri(builder.Configuration[SR.AdminServiceBaseUrl]));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration[SR.AppInsightsConnectionProperty]);

// builder.Configuration to sign-in users with Azure AD B2C

// Azure AD B2C requires scope config with a fully qualified url along with an identifier. To make configuring it more manageable and less
// error prone, we store the names of the scopes separately from the base url with identifier and combine them here.
var adminServiceScopes = builder.Configuration[SR.AdminServiceScopesProperty].Split(" ");
var adminServiceScopeBaseUrl = builder.Configuration[SR.AdminServiceScopeBaseUrlProperty].Trim('/');
for (var i = 0; i < adminServiceScopes.Length; i++)
{
    adminServiceScopes[i] = String.Format("{0}/{1}", adminServiceScopeBaseUrl, adminServiceScopes[i].Trim('/'));
}

// Set the newly-constructed form into memory for lookup when contacting Azure AD B2C later
builder.Configuration[SR.AdminServiceScopesProperty] = string.Join(' ', adminServiceScopes);

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C)
    .EnableTokenAcquisitionToCallDownstreamApi(adminServiceScopes)
    .AddSessionTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

// This is required for auth to work correctly when running in a docker container because of SSL Termination
// Remove this and the subsequent app.UseForwardedHeaders() line below if you choose to run the app without using containers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
});
// Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.AddOptions();
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection(SR.AzureAdB2CProperty));
// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // default
    endpoints.MapControllerRoute(name: SR.DefaultName, pattern: SR.MapControllerRoutePattern);

    endpoints.MapRazorPages();
});

AppHttpContext.Services = ((IApplicationBuilder)app).ApplicationServices;

app.Run();