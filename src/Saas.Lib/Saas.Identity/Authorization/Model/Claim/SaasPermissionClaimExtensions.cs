using Saas.Identity.Authorization.Model.Data;
using Saas.Identity.Authorization.Model.Kind;

namespace Saas.Identity.Authorization.Model.Claim;

public static class SaasPermissionClaimExtensions
{
    public static bool HasSaasUserPermission(this IEnumerable<System.Security.Claims.Claim> claims, UserPermissionKind userPermission) =>
        claims.Where(claim => claim.Type == SaasPermissionClaim<UserPermissionKind>.PermissionClaimsIdentifier)
            .Select(claim => new SaasPermissionClaim<UserPermissionKind>(claim.Value, UserPermission.EntityName))
            .Any(permission => permission.Permission == userPermission);

    public static bool HasSaasUserPermissionSelf(this IEnumerable<System.Security.Claims.Claim> claims) =>
        claims.HasSaasUserPermission(UserPermissionKind.Self);

    public static bool HasSaasTenantPermission(this IEnumerable<System.Security.Claims.Claim> claims, TenantPermissionKind tenantPermission) =>
        claims.Where(claim => claim.Type == SaasPermissionClaim<UserPermissionKind>.PermissionClaimsIdentifier)
            .Select(claim => new SaasPermissionClaim<TenantPermissionKind>(claim.Value, TenantPermission.EntityName))
            .Any(permission => permission.Permission == tenantPermission);

    public static bool HasSaasTenantPermissionAdmin(this IEnumerable<System.Security.Claims.Claim> claims) =>
        claims.HasSaasTenantPermission(TenantPermissionKind.Admin);
}
