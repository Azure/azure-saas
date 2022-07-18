using Microsoft.Extensions.Options;
using Saas.Permissions.Service.Models.AppSettings;
namespace Saas.Permissions.Service.Utilities;

public class ApiKeyMiddleware {
    private readonly RequestDelegate _next;
    private const string API_KEY = "x-api-key";
    private readonly AppSettings _appSettings; 
    public ApiKeyMiddleware(IOptions<AppSettings> appSettings, RequestDelegate next) {
        _next = next;
        _appSettings = appSettings.Value;
    }
    public async Task InvokeAsync(HttpContext context) {

        if (!context.Request.Headers.TryGetValue(API_KEY, out var extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync($"API Key must be provided on the ${API_KEY} header");
            return;
        }
        
        var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = _appSettings.ApiKey;
        
        if (!apiKey.Equals(extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key provided was invalid.");
            return;
        }
        await _next(context);
    }
}