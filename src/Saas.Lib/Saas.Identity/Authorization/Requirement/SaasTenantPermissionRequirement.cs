using Microsoft.AspNetCore.Authorization;

namespace Saas.Identity.Authorization.Requirement;

public class SaasTenantPermissionRequirement : IAuthorizationRequirement
{
    public string TypeName { get; init; } = "permissions";

    public string PermissionName { get; init; } = "Tenant";

    public string RouteKeyName { get; init; } = "tenantId";

    public string Permission { get; }

    public SaasTenantPermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
