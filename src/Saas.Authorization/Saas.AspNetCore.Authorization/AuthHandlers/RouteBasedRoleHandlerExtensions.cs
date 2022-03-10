using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Saas.AspNetCore.Authorization.AuthHandlers
{
    public static class RouteBasedRoleHandlerExtensions
    {
        public static IServiceCollection AddRouteBasedRoleHandler(this IServiceCollection services, string routeValueName)
        {
            services.AddOptions<RouteBasedRoleHandlerOptions>()
                .Configure(options =>
                {
                    options.RouteValueName = routeValueName;
                });

            services.RegisterHandler();

            return services;
        }


        private static void RegisterHandler(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, RouteBasedRoleHandler>();
        }
    }
}
