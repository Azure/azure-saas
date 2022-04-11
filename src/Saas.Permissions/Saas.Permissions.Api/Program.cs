using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Saas.Permissions.Api.Data;
using Saas.Permissions.Api.Interfaces;
using Saas.Permissions.Api.Models.AppSettings;
using Saas.Permissions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));


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
builder.Services.AddSingleton<ICertificateValidationService, CertificateValidationService>();


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
                } else
                {
                    context.Fail("Cert Thumbprint is Invalid");
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
        options.TokenValidationParameters.NameClaimType = "name";
    },
    options => { builder.Configuration.Bind("AzureAdB2C", options); });

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
