using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Saas.Admin.Service;
using Saas.Admin.Service.Data;
using Saas.AspNetCore.Authorization.AuthHandlers;
using Saas.AspNetCore.Authorization.ClaimTransformers;
using Saas.Identity.Authorization;
using Saas.Shared.Options;

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
    InitializeDevEnvironment();
}
else
{
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

builder.Services.Configure<ClaimToRoleTransformerOptions>(
        builder.Configuration.GetRequiredSection(ClaimToRoleTransformerOptions.SectionName));


// Using Entity Framework for accessing permission data stored in the Permissions Db.
builder.Services.AddDbContext<TenantsContext>(options =>
{  
    var sqlConnectionString = builder.Configuration.GetRequiredSection(SqlOptions.SectionName)
        .Get<SqlOptions>()?.TenantSQLConnectionString
            ?? throw new NullReferenceException("SQL Connection string cannot be null.");

    options.UseSqlServer(sqlConnectionString);
});

// Add authentication for incoming requests
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, AzureB2CAdminApiOptions.SectionName);

builder.Services.AddTransient<IClaimsTransformation, ClaimPermissionToRoleTransformer>();

// builder.Services.AddRouteBasedRoleHandler("tenantId");
//builder.Services.AddRouteBasedRoleHandler("userId");

builder.Services.AddHttpContextAccessor();

// builder.Services.AddScoped<IRoleCustomizer, RouteBasedRoleCustomizer>();

// TODO (SaaS): Add necessary roles to scopes for SaaS App operations

builder.Services.AddSingleton<IAuthorizationHandler, SaasTenantPermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, SaasTenantPermissionAuthorizationPolicyProvider>();

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy(AppConstants.Policies.Authenticated, policyBuilder =>
//    {
//        policyBuilder.RequireAuthenticatedUser();
//    });

//    options.AddPolicy(AppConstants.Policies.GlobalAdmin, policyBuilder =>
//    {
//        policyBuilder.RequireAuthenticatedUser();
//        policyBuilder.RequireRole(AppConstants.Roles.GlobalAdmin,
//                                  AppConstants.Roles.Self);
//    });

//    options.AddPolicy(AppConstants.Policies.CreateTenant, policyBuilder =>
//    {
//        policyBuilder.RequireAuthenticatedUser();
//    });

//    options.AddPolicy(AppConstants.Policies.TenantGlobalRead, policyBuilder =>
//    {
//        policyBuilder.RequireRole(AppConstants.Roles.GlobalAdmin);
//        policyBuilder.RequireScope(AppConstants.Scopes.GlobalRead);
//    });

//    options.AddPolicy(AppConstants.Policies.TenantRead, policyBuilder =>
//    {
//        policyBuilder.RequireRole(AppConstants.Roles.GlobalAdmin,
//                                  AppConstants.Roles.TenantUser,
//                                  AppConstants.Roles.TenantAdmin);

//        policyBuilder.RequireScope(AppConstants.Scopes.Read,
//                                   AppConstants.Scopes.GlobalRead);
//    });

//    options.AddPolicy(AppConstants.Policies.TenantWrite, policyBuilder =>
//    {
//        policyBuilder.RequireRole(AppConstants.Roles.GlobalAdmin,
//                                  AppConstants.Roles.TenantAdmin);

//        policyBuilder.RequireScope(AppConstants.Scopes.GlobalWrite,
//                                   AppConstants.Scopes.Write);
//    });

//    options.AddPolicy(AppConstants.Policies.TenantDelete, policyBuilder =>
//    {
//        policyBuilder.RequireRole(AppConstants.Roles.GlobalAdmin,
//                                  AppConstants.Roles.TenantAdmin);

//        policyBuilder.RequireScope(AppConstants.Scopes.GlobalDelete,
//                                   AppConstants.Scopes.Delete);
//    });

//});

builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddHttpClient<IPermissionServiceClient, PermissionServiceClient>()
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

var app = builder.Build();

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