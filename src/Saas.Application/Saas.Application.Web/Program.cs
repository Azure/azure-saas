using Saas.Application.Web.Services;
using Saas.Application.Web.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ITenantService, TenantService>(config => 
{
    config.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AdminServiceUrl") ?? "");
});
builder.Services.AddSingleton<ITenantService, TenantService>();

// This is required for auth to work correctly when running in a docker container because of SSL Termination
// Remove this and the subsequent app.UseForwardedHeaders() line below if you choose to run the app without using containers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
});

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
app.UseForwardedHeaders();

//app.UseAuthorization();

app.MapRazorPages();

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.development.json", true)
    .AddEnvironmentVariables()
    .Build();

app.Run();