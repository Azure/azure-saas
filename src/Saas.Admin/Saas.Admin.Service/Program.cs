using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Polly;
using Saas.Admin.Service.Data;
using Saas.Identity.Authorization.Handler;
using Saas.Identity.Authorization.Option;
using Saas.Identity.Authorization.Provider;
using Saas.Permissions.Client;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Services;
using Saas.Shared.Interface;
using Saas.Shared.Options;
using Saas.Identity.Extensions;
using Saas.Identity.Helper;
using Saas.Identity.Interface;
using Saas.Admin.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

string projectName = Assembly.GetCallingAssembly().GetName().Name
    ?? throw new NullReferenceException("Project name cannot be null.");

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger(projectName);

logger.LogInformation("001");

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


    //Add to enable access to key Vault credentials access, neccessary for accessing services such as 
    //Graph API and other Microsoft related services
    builder.Services.AddScoped<IKeyVaultCredentialService, DevelopmentKeyVaultCredentials>();

    InitializeDevEnvironment();
}
else
{
    //Add to enable access to key Vault credentials access, neccessary for accessing services such as 
    //Graph API and other Microsoft related services
    builder.Services.AddScoped<IKeyVaultCredentialService, ProductionKeyVaultCredentials>();

    InitializeProdEnvironment();
}

builder.Services.Configure<AzureB2CAdminApiOptions>(
        builder.Configuration.GetRequiredSection(AzureB2CAdminApiOptions.SectionName));

builder.Services.Configure<AzureB2CPermissionsApiOptions>(
        builder.Configuration.GetRequiredSection(AzureB2CPermissionsApiOptions.SectionName));

builder.Services.Configure<PermissionsApiOptions>(
        builder.Configuration.GetRequiredSection(PermissionsApiOptions.SectionName));

builder.Services.Configure<SqlOptions>(
            builder.Configuration.GetRequiredSection(SqlOptions.SectionName));

builder.Services.Configure<SaasAuthorizationOptions>(
    builder.Configuration.GetRequiredSection(SaasAuthorizationOptions.SectionName));

//Contains important configuratins that is used to access graph services.
//Such as users profile information
builder.Services.Configure<MSGraphOptions>(
            builder.Configuration.GetRequiredSection(MSGraphOptions.SectionName));

builder.Services.AddHttpContextAccessor();

// Add authentication for incoming requests
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, AzureB2CAdminApiOptions.SectionName);

// Register authorization handlers for authorization
builder.Services.AddSingleton<IAuthorizationHandler, SaasTenantPermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SaasUserPermissionAuthorizationHandler>();

// Register the policy provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, SaasPermissionAuthorizationPolicyProvider>();

builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();


//Provides functionality to register and onboard a system admin user into the systems
builder.Services.AddScoped<ISadUserService>( sp =>
{
    SqlOptions sqlOptions = builder.Configuration.GetRequiredSection(SqlOptions.SectionName).Get<SqlOptions>()??new SqlOptions();


    HashOptions hashes = builder.Configuration.GetRequiredSection(HashOptions.SectionName).Get<HashOptions>() ?? new HashOptions();
    //Should obtain this salt 
    string hashSalt = hashes.PasswordHash ?? string.Empty;

    return string.IsNullOrEmpty(hashSalt) ?
        throw new ArgumentNullException("password hash salt cannot be null")
        : new SadUserService(sqlOptions, hashSalt);
    
});

//Configure graph settings and access rights
builder.Services
    .AddSaasApiCertificateClientCredentials<ISaasMicrosoftGraphApi, AzureB2CPermissionsApiOptions>()
    .AddMicrosoftGraphAuthenticationProvider()
    .AddHttpClient<IGraphApiClientFactory, GraphApiClientFactory>()
    .AddTransientHttpErrorPolicy(builder =>
        builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Adding the service used when accessing MS Graph.
builder.Services.AddScoped<IUserGraphService, UserGraphService>();


builder.Services.AddHttpClient<IPermissionsServiceClient, PermissionsServiceClient>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        using var scope = serviceProvider.CreateScope();

        var baseUrl = scope.ServiceProvider.GetRequiredService<IOptions<AzureB2CPermissionsApiOptions>>().Value.BaseUrl
            ?? throw new NullReferenceException("Permissions Base Url cannot be null");

        var apiKey = scope.ServiceProvider.GetRequiredService<IOptions<PermissionsApiOptions>>().Value.ApiKey
            ?? throw new NullReferenceException("Permissions Base Api Key cannot be null");

        client.BaseAddress = new Uri(baseUrl);

        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    });

// Using Entity Framework for accessing permission data stored in the Permissions Db.
builder.Services.AddDbContext<TenantsContext>(options =>
{
    var sqlConnectionString = builder.Configuration.GetRequiredSection(SqlOptions.SectionName)
        .Get<SqlOptions>()?.TenantSQLConnectionString
            ?? throw new NullReferenceException("SQL Connection string cannot be null.");

    options.UseSqlServer(sqlConnectionString);
});

var app = builder.Build();

////
//Enable CORS for specified endpoints/servers
app.UseCors(ops =>
{
    string[] origins = {
                        "http://localhost:3000",
                        "http://localhost:3000/",
                        "https://192.168.1.5:3000",
                        "https://192.168.1.5:3000/",
                        "https://localhost:3000",
                        "https://localhost:3000/",
                        "https://192.168.1.13:3000",
                        "https://192.168.1.13:3000/"
                    };

    ops.WithOrigins(origins).AllowCredentials().WithMethods("POST", "GET", "PUT", "DELETE").AllowAnyHeader();
});

//Call this as early as possible to make sure DB is ready
//In a larger project it's better update the database during deployment process
app.ConfigureDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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
        option.SwaggerDoc("v1", new() { Title = "Admin API", Version = "v1.1" });
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