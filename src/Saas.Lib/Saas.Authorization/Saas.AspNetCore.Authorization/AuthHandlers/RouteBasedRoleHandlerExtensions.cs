using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Saas.AspNetCore.Authorization.AuthHandlers;

public static class RouteBasedRoleHandlerExtensions
{
    public static IServiceCollection AddRouteBasedRoleHandler(this IServiceCollection services, string routeValueName)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IAuthorizationHandler>(serviceprovider =>
        {
            IHttpContextAccessor httpContextAccessor = serviceprovider.GetRequiredService<IHttpContextAccessor>();
            RouteBasedRoleCustomizer customizer = new(httpContextAccessor, routeValueName);
            CustomRoleHandler customRoleHandler = new(customizer);
            return customRoleHandler;
        });

        return services;
    }
}
