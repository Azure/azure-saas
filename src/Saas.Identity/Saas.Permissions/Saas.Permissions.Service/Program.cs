using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models.AppSettings;
using Saas.Permissions.Service.Services;
using Saas.Permissions.Service.Utilities;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Get Secrets From Azure Key Vault if in production. If not in production, secrets are automatically loaded in from the .NET secrets manager
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0

    // We don't want to fetch all the secrets for the other microservices in the app/solution, so we only fetch the ones with the prefix of "permissions-".
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix

    builder.Configuration.AddAzureKeyVault(
        new Uri(builder.Configuration["KeyVault:Url"]), 
        new DefaultAzureCredential(), 
        new CustomPrefixKeyVaultSecretManager("permissions"));
}

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
builder.Services.Configure<AzureADB2COptions>(builder.Configuration.GetSection("AzureAdB2C"));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PermissionsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PermissionsContext"));
});

builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddScoped<IGraphAPIService, GraphAPIService>();
builder.Services.AddSingleton<ICertificateValidationService, CertificateValidationService>();

// Look for certificate forwarded by the web server on X-Arr-Client-Cert
builder.Services.AddCertificateForwarding(options => { options.CertificateHeader = "X-ARR-ClientCert"; });

// This is required for auth to work correctly when running in a docker container because of SSL Termination
// Remove this and the subsequent app.UseForwardedHeaders() line below if you choose to run the app without using containers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // Add Certificate Validation for authentication from azure b2c.
    .AddCertificate(options =>
    {
        // It is not reccomended to use self signed certificates for production scenarios.
        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-6.0#configure-certificate-validation  
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                var validationService = context.HttpContext.RequestServices
                .GetRequiredService<ICertificateValidationService>();

                if (validationService.ValidateCertificate(context.ClientCertificate))
                {
                    context.Success();
                }
                else
                {
                    context.Fail("Cert Thumbprint is Invalid");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{

});


var app = builder.Build();
app.ConfigureDatabase();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-6.0#configure-certificate-validation
app.UseCertificateForwarding();
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
