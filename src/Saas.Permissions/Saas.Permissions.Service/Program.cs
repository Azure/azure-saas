using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Azure.Identity;
using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models.AppSettings;
using Saas.Permissions.Service.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Get Secrets From Azure Key Vault if in production. If not in production, secrets are automatically loaded in from the .NET secrets manager
    // https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0
    builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVault:Url"]), new DefaultAzureCredential());
}

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Saas.Permissions.Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddDbContext<PermissionsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PermissionsContext"));
});

builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddSingleton<ICertificateValidationService, CertificateValidationService>();

// Look for certificate forwarded by the web server on X-Arr-Client-Cert
builder.Services.AddCertificateForwarding(options => { options.CertificateHeader = "X-ARR-ClientCert"; });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
