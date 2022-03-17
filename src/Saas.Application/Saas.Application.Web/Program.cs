using Saas.Application.Web.Services;
using Saas.Application.Web.Interfaces;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ITenantService, TenantService>(config => 
{
    config.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AdminServiceUrl") ?? "");
});
builder.Services.AddSingleton<ITenantService, TenantService>();  

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.MapRazorPages();

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.development.json", true)
    .AddEnvironmentVariables()
    .Build();

app.Run();