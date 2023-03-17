using Microsoft.AspNetCore.Authorization;
using Saas.Identity.Authorization.Model;

namespace Saas.Identity.Authorization.Requirement;
public interface ISaasRequirement : IAuthorizationRequirement
{
    static abstract string PermissionEntityName { get; }

    SaasPolicy Policy { get; }
    int PermissionValue();
}
