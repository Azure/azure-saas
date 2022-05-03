using Saas.Admin.Service.Data;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;
using Saas.AspNetCore.Authorization.ClaimTransformers;
using Saas.AspNetCore.Authorization.AuthHandlers;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

X509Certificate2 permissionsApiCertificate;

if (builder.Environment.IsProduction())
{
    // Get Secrets From Azure Key Vault if in production. If not in production, secrets are automatically loaded in from the .NET secrets manager
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0
    builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVault:Url"]), new DefaultAzureCredential());

    // Use azure keyvault SDK to download certificate to be used to authenticate with permissions api
    CertificateClient certificateClient = new CertificateClient(new Uri(builder.Configuration["KeyVault:Url"]), new DefaultAzureCredential());
    permissionsApiCertificate = certificateClient.DownloadCertificate(builder.Configuration["KeyVault:PermissionsApiCertName"]).Value;
}
else 
{
    // If running locally, you must first set the certificate as a base 64 encoded string in your .NET secrets manager.
    var certString = builder.Configuration["PermissionsApi:LocalCertificate"];
    permissionsApiCertificate = new X509Certificate2(Convert.FromBase64String(certString));
}

builder.Services.AddDbContext<TenantsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenantsContext")));

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<PermissionsApiOptions>(builder.Configuration.GetSection("PermissionsApi"));


// Add authentication for incoming requests
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");



builder.Services.AddClaimToRoleTransformer(builder.Configuration, "ClaimToRoleTransformer");
builder.Services.AddRouteBasedRoleHandler("tenantId");

builder.Services.AddAuthorization(options => {

    options.AddPolicy("Authenticated", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole("GlobalAdmin", "Self");
    });

    options.AddPolicy("Create_Tenant", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
    });

    options.AddPolicy("Tenant_Global_Read", policyBuilder =>
    {
        policyBuilder.RequireRole("GlobalAdmin");
        policyBuilder.RequireScope("tenant.global.read");
    });

    options.AddPolicy("Tenant_Read", policyBuilder =>
    {
        policyBuilder.RequireRole("GlobalAdmin", "TenantUser", "TenantAdmin");
        policyBuilder.RequireScope("tenant.read tenant.global.read");
    });

    options.AddPolicy("Tenant_Write", policyBuilder =>
    {
        policyBuilder.RequireRole("GlobalAdmin", "TenantAmdin");
        policyBuilder.RequireScope("tenant.write tenant.global.write");
    });

    options.AddPolicy("Tenant_Delete", policyBuilder =>
    {
        policyBuilder.RequireRole("GlobalAdmin", "TenantAdmin");
        policyBuilder.RequireScope("tenant.delete tenant.global.delete");
    });

});


builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddHttpClient<IPermissionServiceClient, PermissionServiceClient>()
    // Configure outgoing HTTP requests to include certificate for permissions API
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        HttpClientHandler handler = new HttpClientHandler();
        handler.ClientCertificates.Add(permissionsApiCertificate);
        return handler;
    })
    .ConfigureHttpClient(options => {
        options.BaseAddress = new Uri(builder.Configuration["PermissionsApi:BaseUrl"]);

        if (builder.Environment.IsDevelopment())
        {
            // The permissions API expects the certificate to be provided to the application layer by the web server after the TLS handshake
            // Since this doesn't happen locally, we need to do it ourselves
            
            options.DefaultRequestHeaders.Add("X-ARR-ClientCert", Convert.ToBase64String(permissionsApiCertificate.GetRawCertData()));
        }
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string? xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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