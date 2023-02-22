using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Saas.Identity.Authorization.Requirement;

namespace Saas.Identity.Authorization;
public class SaasTenantPermissionAuthorizationHandler : AuthorizationHandler<SaasTenantPermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SaasTenantPermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SaasTenantPermissionRequirement requirement)
    {
        string? routeValue = _httpContextAccessor?.HttpContext?.GetRouteValue(requirement.RouteKeyName) as string;

        HashSet<string> permissions = context.User.Claims
            .Where(x => x.Type == requirement.TypeName)
            .Select(x => x.Value.Split('.'))
            .Where(x => x.First().Equals(requirement.PermissionName))
            .Where(x => x.Skip(1).First().Equals(routeValue) || x.Skip(1).First().Equals("Global"))
            .Select(x => x.Last())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToHashSet();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
