using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Sass.AspNetCore.Authorization.PolicyRequirements
{
    public static class RouteBasedPolicyExtensions
    {
        public static IServiceCollection AddRouteBasedPolicy(this IServiceCollection services)

        {
            services.AddSingleton<IAuthorizationHandler, RouteBasedPolicyHandler>();
            return services;
        }
    }
}
