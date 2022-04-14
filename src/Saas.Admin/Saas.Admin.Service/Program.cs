using Saas.Admin.Service.Data;
using Microsoft.Identity.Web;
using Saas.Admin.Service.Data.AppSettings;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TenantsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenantsContext")));

// Add options using options pattern : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
builder.Services.Configure<PermissionsApiOptions>(builder.Configuration.GetSection("PermissionsApi"));


// Add authentication for incoming requests
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAd", options);

            options.TokenValidationParameters.NameClaimType = "name";
        },
        options => { builder.Configuration.Bind("AzureAd", options); })
        // Inject token acquisition service to allow for token requests for permissions api
        .EnableTokenAcquisitionToCallDownstreamApi(options => {})
        .AddDownstreamWebApi("PermissionsApi", builder.Configuration.GetSection("PermissionsApi"))
        .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options => { });


builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddHttpClient<IPermissionServiceClient, PermissionServiceClient>()
    .ConfigureHttpClient(options => options.BaseAddress = new Uri(builder.Configuration["PermissionsApi:BaseUrl"]) );

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

