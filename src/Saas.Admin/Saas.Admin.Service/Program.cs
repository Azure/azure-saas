using Saas.Admin.Service.Data;
using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TenantsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenantsContext")));

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<PermissionsApiOptions>(builder.Configuration.GetSection("PermissionsApi"));


// Add authentication for incoming requests
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");


builder.Services.AddAuthorization(options => { });


builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Use azure keyvault SDK to download certificate to be used to authenticate with permissions api
CertificateClient certificateClient = new CertificateClient(new Uri(builder.Configuration["KeyVault:Url"]), new DefaultAzureCredential());
X509Certificate2 certificate = certificateClient.DownloadCertificate(builder.Configuration["KeyVault:PermissionsApiCertName"]).Value;

builder.Services.AddHttpClient<IPermissionServiceClient, PermissionServiceClient>()
    // Configure outgoing HTTP requests to include certificate for permissions API
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        HttpClientHandler handler = new HttpClientHandler();
        handler.ClientCertificates.Add( certificate );
        return handler;
    })
    .ConfigureHttpClient(options => {
        options.BaseAddress = new Uri(builder.Configuration["PermissionsApi:BaseUrl"]);

        if (builder.Environment.IsDevelopment())
        {
            // The permissions API expects the certificate to be provided to the application layer by the web server after the TLS handshake
            // Since this doesn't happen locally, we need to do it ourselves
            var certData = Convert.ToBase64String(certificate.GetRawCertData());
            options.DefaultRequestHeaders.Add("X-ARR-ClientCert", certData);
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

