using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Saas.AspNetCore.Authorization.AuthHandlers;

public class CustomRoleHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    private readonly IRoleCustomizer _customizer;

    public CustomRoleHandler(IRoleCustomizer customizer)
    {
        _customizer = customizer;

    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
    {
        List<string> customRoles = _customizer.CustomizeRoles(requirement.AllowedRoles).ToList();

        if (customRoles is not null)
        {
            if (customRoles.Any(role => context.User.IsInRole(role)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        return Task.CompletedTask;
    }
}
