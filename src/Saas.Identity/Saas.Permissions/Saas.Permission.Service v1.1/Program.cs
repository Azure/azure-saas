using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Options;
using Saas.Permissions.Service.Services;
using Saas.Permissions.Service.Swagger;
using ClientAssertionWithKeyVault.Interface;
using ClientAssertionWithKeyVault;
using Saas.Permissions.Service.Middleware;

var builder = WebApplication.CreateBuilder(args);

/* IMPORTANT
In the configuration pattern used here, we're trying to minimize the use of appsettings.json, and well as eliminate the need for 
storing local secrets entirely. Instead we're utilizing the Azure App Configuration service for storing settings 
and Azure Key Vault to store secrets. This approach is more secure, and allows us to have a single source of truth 
for all settings and secrets. 

The settings and secrets have been provisioned for Production during the deployment of the Identity Framework.

For local development, please see the ASDK Permission Service README.md for more instructions on how to set up the local environment. 
*/


if (builder.Environment.IsDevelopment())
{
    // The current version. Must corresspond to the version of our deployment as specificed in the deployment config.json.
    var version = "ver0.8.0";

    // For local development, use the Secret Manager feature of .NET to store a connection string
    // and likewise for storing a secret for the permission-api app. 
    // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows

    var connectionString = builder.Configuration.GetConnectionString("AppConfig")
        ?? throw new NullReferenceException("App config missing.");

    // Use the connection string to access Azure App Configuration to get access to app settings stored there.
    // To gain access to Azure Key Vault use 'Azure Cli: az login' to log into Azure.
    // This login on will also now provide valid access tokens to the local development environment.
    // For more details and the option to chain and combine multiple credential options with `ChainedTokenCredential`
    // plese see: https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#define-a-custom-authentication-flow-with-chainedtokencredential
    builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(connectionString)
                .ConfigureKeyVault(kv => kv.SetCredential(new ChainedTokenCredential(
                    new AzureCliCredential())))
            .Select(KeyFilter.Any, version)); // <-- Important: because we're using labels in our Azure App Configuration store

    // Enabling to option for add the 'x-api-key' header to swagger UI.
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new() { Title = "Permissions API", Version = "v1.1" });
        option.OperationFilter<SwagCustomHeaderFilter>();
    });
}
else
{
    // For procution environment, we'll configured Managed Identities for managing access Azure App Services
    // and Key Vault. The Azure App Services endpoint is stored in an environment variable for the web app.
    
    var appConfigEndpoint = builder.Configuration.GetSection("AppConfiguration:Endpoint")?.Value
        ?? throw new NullReferenceException("The Azure App Configuration Endpoint cannot be found. Has the endpoint environment variable been set correctly for the Web App?");

    var version = builder.Configuration.GetSection("Version")?.Value
        ?? throw new NullReferenceException("The Version value cannot be found. Has the 'Version' environment variable been set correctly for the Web App?");

    builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(appConfigEndpoint)
                .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
            .Select(KeyFilter.Any, version)); // <-- Important because we're using labels in our Azure App Configuration store
}

// Add configuration settings data  using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
// builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
builder.Services
    .Configure<PermissionApiOptions>(
        builder.Configuration.GetSection(PermissionApiOptions.SectionName));

builder.Services
    .Configure<SqlOptions>(
        builder.Configuration.GetSection(SqlOptions.SectionName));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PermissionsContext>(options =>
{
    var sqlOptions = builder.Configuration.GetSection(SqlOptions.SectionName).Get<SqlOptions>()
        ?? throw new NullReferenceException("SQL Connection string cannot be null.");

    options.UseSqlServer(sqlOptions.SQLConnectionString);
});

builder.Services.AddScoped<IPermissionsService, PermissionsService>();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IGraphAPIService, GraphAPIService>();

// Custom auth provider for obtaining an access token for the Permission API to make requests to MS Graph.
builder.Services.AddSingleton<Microsoft.Graph.IAuthenticationProvider, CertificateCredentialsAuthProvider>();

// These two are used fetch the public key data we need for signing a client assertion...
builder.Services.AddSingleton<IPublicX509CertificateDetailProvider, PublicX509CertificateDetailProvider>();
// ... and getting it signed by Azure Key Vault
builder.Services.AddSingleton<IClientAssertionSigningProvider, ClientAssertionSigningProvider>();

var app = builder.Build();

app.ConfigureDatabase();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Saas Permissions Service 1.1");
    });
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();

// Adds middleware to check for the presence of a valid API Key
// For debugging purposes, you can comment out 'app.UseMiddleware...'. This way you
// don't have to add the secret to the header everytime you want to test something in swagger, for instance.
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
