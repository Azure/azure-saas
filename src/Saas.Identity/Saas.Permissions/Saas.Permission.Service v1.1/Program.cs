using Azure.Identity;
using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Options;
using Saas.Permissions.Service.Services;
using Saas.Permissions.Service.Swagger;
using ClientAssertionWithKeyVault.Interface;
using ClientAssertionWithKeyVault;
using Saas.Permissions.Service.Middleware;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;
using Polly;

var builder = WebApplication.CreateBuilder(args);

/* IMPORTANT
In the configuration pattern used here, we're seeking to minimize the use of appsettings.json, 
and well as eliminate the need for storing local secrets entirely. Instead we're utilizing
the Azure App Configuration service for storing settings and Azure Key Vault to store secrets. 

This approach is more secure, and allows us to have a single source of truth 
for all settings and secrets. 

The settings and secrets were provisioned to Azure App Configuration and Azure Key Vault 
during the deployment of the Identity Framework.

For local development, please see the ASDK Permission Service README.md for more 
instructions on how to set up the local environment. 
*/


if (builder.Environment.IsDevelopment())
{
    // IMPORTANT
    // The current version.
    // Must corresspond exactly to the version string of our deployment as specificed in the deployment config.json.
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
            .Select(KeyFilter.Any, version)); // <-- Important: since we're using labels in our Azure App Configuration store

    // Enabling to option for add the 'x-api-key' header to swagger UI.
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new() { Title = "Permissions API", Version = "v1.1" });
        option.OperationFilter<SwagCustomHeaderFilter>();
    });

    // Configuring Swagger.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
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
            .Select(KeyFilter.Any, version)); // <-- Important since we're using labels in our Azure App Configuration store
}

// Add configuration settings data  using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
// builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
builder.Services
    .Configure<PermissionApiOptions>(
        builder.Configuration.GetSection(PermissionApiOptions.SectionName));

builder.Services
    .Configure<SqlOptions>(
            builder.Configuration.GetSection(SqlOptions.SectionName));

    builder.Services
    .Configure<MSGraphOptions>(
            builder.Configuration.GetSection(MSGraphOptions.SectionName));


builder.Services.AddControllers();

// Using Entity Framework for accessing permission data stored in the Permissions Db.
builder.Services.AddDbContext<PermissionsContext>(options =>
{
    var sqlOptions = builder.Configuration.GetSection(SqlOptions.SectionName).Get<SqlOptions>()
        ?? throw new NullReferenceException("SQL Connection string cannot be null.");

    options.UseSqlServer(sqlOptions.SQLConnectionString);
});

// Adding the permission service used by the API controller
builder.Services.AddScoped<IPermissionsService, PermissionsService>();

// Adding memory cache, which is used to cache credentials optained from Azure Key Vault.
builder.Services.AddMemoryCache();

// Custom auth provider for obtaining an access token for the Permission API to make requests to MS Graph.
builder.Services.AddSingleton<Microsoft.Graph.IAuthenticationProvider, KeyVaultSigningCredentialsAuthProvider>();

// These two are used fetch the public key data we need for signing a client assertion...
builder.Services.AddSingleton<IPublicX509CertificateDetailProvider, PublicX509CertificateDetailProvider>();

// ... and getting it signed by Azure Key Vault.

builder.Services.AddSingleton<IClientAssertionSigningProvider, ClientAssertionSigningProvider>();
// Both are made singletons to ensure that data is cached after first request.


// Create a httpClient using HttpClientFactory for MS Graph requests, which provides the ability to use Polly
// For more see: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
// and see: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
builder.Services.AddHttpClient<IGraphApiClientFactory, GraphApiClientFactory>()
    .AddTransientHttpErrorPolicy(builder => 
        builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Adding the service used to access MS Graph.
builder.Services.AddScoped<IGraphAPIService, GraphAPIService>();

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

// Adds middleware to check for the presence of a valid API Key
// For debugging purposes, you can comment out 'app.UseMiddleware...'. This way you
// don't have to add the secret to the header everytime you want to test something in swagger, for instance.
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
