﻿using Azure.Identity;
using Microsoft.Identity.Web.UI;
using Saas.Application.Web;
using Saas.Application.Web.Interfaces;
using System.Reflection;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Saas.Shared.Options;
using Saas.Identity.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Saas.Identity.Helper;
using Saas.SignupAdministration.Web.Services;
using Saas.Admin.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

string projectName = Assembly.GetCallingAssembly().GetName().Name
    ?? throw new NullReferenceException("Project name cannot be null.");

var version = builder.Configuration.GetRequiredSection("Version")?.Value
        ?? throw new NullReferenceException("The Version value cannot be found. Has the 'Version' environment variable been set correctly for the Web App?");

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger(projectName);

logger.LogInformation("Debug edition: 001");
logger.LogInformation("Version: {version}", version);

/*  IMPORTANT
    In the configuration pattern used here, we're seeking to minimize the use of appsettings.json, 
    as well as eliminate the need for storing local secrets. 

    Instead we're utilizing the Azure App Configuration service for storing settings and the Azure Key Vault to store secrets.
    Azure App Configuration still hold references to the secret, but not the secret themselves.

    This approach is more secure and allows us to have a single source of truth 
    for all settings and secrets. 

    The settings and secrets are provisioned by the deployment script made available for deploying this service.
    Please see the readme for the project for details.

    For local development, please see the ASDK Permission Service readme.md for more 
    on how to set up and run this service in a local development environment - i.e., a local dev machine. 
*/

if (builder.Environment.IsDevelopment())
{
    InitializeDevEnvironment();
}
else
{
    InitializeProdEnvironment();
}

builder.Services.Configure<AzureB2CSaasAppOptions>(
        builder.Configuration.GetRequiredSection(AzureB2CSaasAppOptions.SectionName));

builder.Services.AddRazorPages();

builder.Services.AddMvc();
// Add this to allow for context to be shared outside of requests
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Get Application Id Uri for Admin Api.
var applicationIdUri = builder.Configuration.GetRequiredSection(AdminApiOptions.SectionName)
    .Get<AdminApiOptions>()?.ApplicationIdUri
        ?? throw new NullReferenceException($"ApplicationIdUri cannot be null");

// Get scope names some Admin Api
var scopes = new[] { "tenant.read" };
//var scopes = builder.Configuration.GetRequiredSection(AdminApiOptions.SectionName)
//    .Get<AdminApiOptions>()?.Scopes
//        ?? throw new NullReferenceException("Scopes cannot be null");

// Azure AD B2C requires scope config with a fully qualified url along with an identifier. To make configuring it more manageable and less
// error prone, we store the names of the scopes separately from the application id uri and combine them when neded.
var fullyQualifiedScopes = scopes.Select(scope => $"{applicationIdUri}/{scope}".Trim('/'));

// Adding SaaS Authentication and setting web app up for calling the Admin API
builder.Services.AddSaasWebAppAuthentication(AzureB2CSaasAppOptions.SectionName, builder.Configuration, fullyQualifiedScopes)
    .SaaSAppCallDownstreamApi()
    .AddInMemoryTokenCaches();

// Managing the situation where the access token is not in cache.
// For more details please see: https://github.com/AzureAD/microsoft-identity-web/issues/13
builder.Services.Configure<CookieAuthenticationOptions>(
    CookieAuthenticationDefaults.AuthenticationScheme,
    options => options.Events = new RejectSessionCookieWhenAccountNotInCacheEvents(fullyQualifiedScopes));

builder.Services.AddHttpClient<IAdminServiceClient, AdminServiceClient>(httpClient =>
{
    string adminApiBaseUrl = builder.Environment.IsDevelopment()
        ? builder.Configuration.GetRequiredSection("adminApi:baseUrl").Value
            ?? throw new NullReferenceException("Environment is running in development mode. Please specify the value for 'adminApi:baseUrl' in appsettings.json.")
        : builder.Configuration.GetRequiredSection(AzureB2CAdminApiOptions.SectionName)?.Get<AzureB2CAdminApiOptions>()?.BaseUrl
            ?? throw new NullReferenceException($"{nameof(AzureB2CAdminApiOptions)} Url cannot be null");

    httpClient.BaseAddress = new Uri(adminApiBaseUrl);
});

// Required for the JsonPersistenceProvider
// Should be replaced based on the persistence scheme
builder.Services.AddMemoryCache();

// TODO (SaaS): Replace with your implementation of persistence provider
// Session persistence is the default
builder.Services.AddScoped<IPersistenceProvider, JsonSessionPersistenceProvider>();

// Add the user details that come back from B2C
builder.Services.AddScoped<IApplicationUser, ApplicationUser>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();



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
// app.UseForwardedHeaders();
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});

app.MapControllerRoute(name: SR.DefaultName, pattern: SR.MapControllerRoutePattern);
app.MapRazorPages();

AppHttpContext.Services = ((IApplicationBuilder)app).ApplicationServices;

app.Run();

/*---------------
  local methods
----------------*/

void InitializeDevEnvironment()
{
    // IMPORTANT
    // The current version.
    // Must corresspond exactly to the version string of our deployment as specificed in the deployment config.json.

    logger.LogInformation($"Is Development.");

    // For local development, use the Secret Manager feature of .NET to store a connection string
    // and likewise for storing a secret for the permission-api app. 
    // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows

    var appConfigurationconnectionString = builder.Configuration.GetConnectionString("AppConfig")
        ?? throw new NullReferenceException("App config missing.");

    // Use the connection string to access Azure App Configuration to get access to app settings stored there.
    // To gain access to Azure Key Vault use 'Azure Cli: az login' to log into Azure.
    // This login on will also now provide valid access tokens to the local development environment.
    // For more details and the option to chain and combine multiple credential options with `ChainedTokenCredential`
    // please see: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#define-a-custom-authentication-flow-with-chainedtokencredential

    AzureCliCredential credential = new();

    builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(appConfigurationconnectionString)
                .ConfigureKeyVault(kv => kv.SetCredential(credential))
            .Select(KeyFilter.Any, version)); // <-- Important: since we're using labels in our Azure App Configuration store

    logger.LogInformation($"Initialization complete.");
}

void InitializeProdEnvironment()
{
    // For procution environment, we'll configured Managed Identities for managing access Azure App Services
    // and Key Vault. The Azure App Services endpoint is stored in an environment variable for the web app.

    logger.LogInformation($"Is Production.");

    var appConfigurationEndpoint = builder.Configuration.GetRequiredSection("AppConfiguration:Endpoint")?.Value
        ?? throw new NullReferenceException("The Azure App Configuration Endpoint cannot be found. Has the endpoint environment variable been set correctly for the Web App?");

    // Get the ClientId of the UserAssignedIdentity
    // If we don't set this ClientID in the ManagedIdentityCredential constructor, it doesn't know it should use the user assigned managed id.
    var managedIdentityClientId = builder.Configuration.GetRequiredSection("UserAssignedManagedIdentityClientId")?.Value
        ?? throw new NullReferenceException("The Environment Variable 'UserAssignedManagedIdentityClientId' cannot be null. Check the App Service Configuration.");

    ManagedIdentityCredential userAssignedManagedCredentials = new(managedIdentityClientId);

    builder.Configuration.AddAzureAppConfiguration(options =>
        options.Connect(new Uri(appConfigurationEndpoint), userAssignedManagedCredentials)
            .ConfigureKeyVault(kv => kv.SetCredential(userAssignedManagedCredentials))
        .Select(KeyFilter.Any, version)); // <-- Important since we're using labels in our Azure App Configuration store
}