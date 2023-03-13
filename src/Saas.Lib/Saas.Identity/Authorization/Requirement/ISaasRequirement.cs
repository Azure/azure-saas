using Microsoft.AspNetCore.Authorization;
using Saas.Identity.Authorization.Model;

namespace Saas.Identity.Authorization.Requirement;
public interface ISaasRequirement : IAuthorizationRequirement
{
    static abstract string PermissionEntityName { get; }

    //static string PermissionClaimsIdentifier { get; } = "permissions";
    SaasPolicy Policy { get; }
    int PermissionValue();
}
