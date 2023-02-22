using Microsoft.AspNetCore.Authorization;
using Saas.Identity.Authorization.Enum;

namespace Saas.Identity.Authorization.Attribute;
public class SaaSUserPermissionsAttribute : AuthorizeAttribute
{
    public SaaSUserPermissionsAttribute(SaaSUserPermission saasUserPermission)
    {
        base.Policy = saasUserPermission.ToString();
    }
}
