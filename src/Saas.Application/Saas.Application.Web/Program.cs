var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

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