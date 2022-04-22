using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

using Saas.AspNetCore.Authorization.AuthHandlers;
using Saas.AspNetCore.Authorization.ClaimTransformers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));


builder.Services.AddHttpContextAccessor();

builder.Services.AddClaimToRoleTransformer(builder.Configuration, "ClaimToRoleTransformer");
builder.Services.AddRouteBasedRoleHandler("subscriptionId");


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;

    options.AddPolicy("AdminsOnlyPolicy", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new string[] { "SubscriptionAdmin", "SystemAdmin" }));
    });

    options.AddPolicy("SubscriptionAdminOnly", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new string[] { "SubscriptionAdmin" }));
    });

    options.AddPolicy("SuperAdminOnly", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new string[] { "SuperAdmin" }));
    });

    options.AddPolicy("SubscriptionUsersOnly", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new string[] { "SubscriptionUser" }));
    });
});


builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();
app.Run();
