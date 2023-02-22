
using Microsoft.AspNetCore.Authorization;
using Saas.Identity.Authorization.Enum;

namespace Saas.Identity.Authorization.Attribute;
public class SaasTenantPermissionsAttribute : AuthorizeAttribute
{
    public SaasTenantPermissionsAttribute(SaasTenantPermission saasTenantPermission)
    {
        base.Policy = saasTenantPermission.ToString();
    }
}
