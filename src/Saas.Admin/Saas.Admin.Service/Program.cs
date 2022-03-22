var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TenantsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenantsContext")));

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd")).EnableTokenAcquisitionToCallDownstreamApi()
            .AddDownstreamWebApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();

builder.Services.AddControllers();

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

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

