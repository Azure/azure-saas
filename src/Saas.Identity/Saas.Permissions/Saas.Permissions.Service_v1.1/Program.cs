using Azure.Identity;
using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Interfaces;
using Saas.Shared.Options;
using Saas.Permissions.Service.Services;
using Saas.Permissions.Service.Middleware;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Reflection;
using Saas.Identity.Extensions;
using Saas.Shared.Interface;
using Saas.Identity.Helper;
using Saas.Identity.Interface;
using Polly;
using Saas.Permissions.Service.Data.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

/*  IMPORTANT
    In the configuration pattern used here, we're seeking to minimize the use of appsettings.json, 
    as well as eliminate the need for storing local secrets. 

    Instead we're utilizing the Azure App Configuration service for storing settings and the Azure Key Vault to store secrets.
    Azure App Configuration still hold references to the secret, but not the secret themselves.

    This approach is more secure and allows us to have a single source of truth 
    for all settings and secrets. 

    The settings and secrets were provisioned to Azure App Configuration and Azure Key Vault 
    during the deployment of the Identity Framework.

    For local development, please see the ASDK Permission Service readme.md for more 
    on how to set up and run this service in a local development environment - i.e., a local dev machine. 
*/

string projectName = Assembly.GetCallingAssembly().GetName().Name
    ?? throw new NullReferenceException("Project name cannot be null");

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger(projectName);

logger.LogInformation("001");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IKeyVaultCredentialService, DevelopmentKeyVaultCredentials>();
    InitializeDevEnvironment();
}
else
{
    builder.Services.AddScoped<IKeyVaultCredentialService, ProductionKeyVaultCredentials>();
    InitializeProdEnvironment();
}

// Add configuration settings data using Options Pattern.
// For more see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0
builder.Services.Configure<PermissionsApiOptions>(
        builder.Configuration.GetRequiredSection(PermissionsApiOptions.SectionName));

builder.Services.Configure<AzureB2CPermissionsApiOptions>(
        builder.Configuration.GetRequiredSection(AzureB2CPermissionsApiOptions.SectionName));

builder.Services.Configure<SqlOptions>(
            builder.Configuration.GetRequiredSection(SqlOptions.SectionName));

builder.Services.Configure<MSGraphOptions>(
            builder.Configuration.GetRequiredSection(MSGraphOptions.SectionName));

builder.Services.AddControllers();

// Using Entity Framework for accessing permission data stored in the Permissions Db.
builder.Services.AddDbContext<SaasPermissionsContext>(options =>
{
    var sqlConnectionString = builder.Configuration.GetRequiredSection(SqlOptions.SectionName)
        .Get<SqlOptions>()?.PermissionsSQLConnectionString
            ?? throw new NullReferenceException("SQL Connection string cannot be null.");

    options.UseSqlServer(sqlConnectionString);
});

builder.Services
    .AddSaasApiCertificateClientCredentials<ISaasMicrosoftGraphApi, AzureB2CPermissionsApiOptions>()
    .AddMicrosoftGraphAuthenticationProvider()
    .AddHttpClient<IGraphApiClientFactory, GraphApiClientFactory>()
    .AddTransientHttpErrorPolicy(builder =>
        builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Adding the service used when accessing MS Graph.
builder.Services.AddScoped<IGraphAPIService, GraphAPIService>();

// Adding the permission service used by the API controller
builder.Services.AddScoped<IPermissionsService, PermissionsService>();

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}
else
{
    builder.Services.AddApplicationInsightsTelemetry();
}

var app = builder.Build();

// Configuring the db holding the permissions data.
app.ConfigureDatabase();

// Use Swagger when running in development mode.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Saas Permissions Service 1.1");
    });
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();

if (! app.Environment.IsDevelopment())
{
    // When now in development, add middleware to check for the presaz ence of a valid API Key
    // For debugging purposes, you can comment out 'app.UseMiddleware...'. This way you
    // don't have to add the secret to the header everytime you want to test something in swagger, for instance.
    app.UseMiddleware<ApiKeyMiddleware>();
}

app.MapControllers();

app.Run();


/*---------------
  local methods
----------------*/

void InitializeDevEnvironment()
{
    // IMPORTANT
    // The current version.
    // Must correspond exactly to the version string of our deployment as specificed in the deployment config.json.
    var version = "ver0.8.0";

    logger.LogInformation("Version: {version}", version);
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
                .ConfigureKeyVault(kv => kv.SetCredential(new ChainedTokenCredential(credential)))
            .Select(KeyFilter.Any, version)); // <-- Important: since we're using labels in our Azure App Configuration store

    // Configuring Swagger.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    // Enabling to option for add the 'x-api-key' header to swagger UI.
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new() { Title = "Permissions API", Version = "v1.1" });
        // option.OperationFilter<SwagCustomHeaderFilter>();
    });
}

void InitializeProdEnvironment()
{
    // For procution environment, we'll configured Managed Identities for managing access Azure App Services
    // and Key Vault. The Azure App Services endpoint is stored in an environment variable for the web app.

    var version = builder.Configuration.GetRequiredSection("Version")?.Value
        ?? throw new NullReferenceException("The Version value cannot be found. Has the 'Version' environment variable been set correctly for the Web App?");

    logger.LogInformation("Version: {version}", version);
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

