using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Saas.Identity.Authorization.Model.Claim;
using Saas.Identity.Authorization.Model.Kind;
using Saas.Identity.Authorization.Option;
using Saas.Identity.Authorization.Requirement;
using System.Security.Claims;

namespace Saas.Identity.Authorization.Handler;
public sealed class SaasUserPermissionAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<SaasAuthorizationOptions> saasAuthorizationOptions) 
        : SaasPermissionAuthorizationHandlerBase<SaasUserPermissionRequirement, UserPermissionKind>(httpContextAccessor, saasAuthorizationOptions)
{
    protected override bool IsValidPermission(
        SaasPermissionClaim<UserPermissionKind> permission, 
        AuthorizationHandlerContext context, 
        SaasUserPermissionRequirement requirement)
    {
        var userId = context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            userIdGuid = default;
        }

        return permission.Entity == userIdGuid;
    }
}
