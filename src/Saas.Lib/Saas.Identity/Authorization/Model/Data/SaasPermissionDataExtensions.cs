
namespace Saas.Identity.Authorization.Model.Data;
public static class SaasPermissionDataExtensions
{
    public static SaasPermission IncludeAllPermissionSets(this SaasPermission permission) => new()
    {
        UserId = permission.UserId,
        TenantId = permission.TenantId,
        UserPermissions = permission.UserPermissions.Select(p => new UserPermission
        {
            PermissionStr = p.PermissionStr,
            UserId = permission.UserId
        }).ToList(),
        TenantPermissions = permission.TenantPermissions.Select(p => new TenantPermission
        {
            PermissionStr = p.PermissionStr,
            TenantId = permission.TenantId
        }).ToList()
    };

    public static SaasPermission IncludeUserPermissions(this SaasPermission permission) => new()
    {
        UserId = permission.UserId,
        TenantId = permission.TenantId,
        UserPermissions = permission.UserPermissions.Select(p => new UserPermission
        {
            PermissionStr = p.PermissionStr,
            UserId = permission.UserId
        }).ToList()
    };

    public static SaasPermission IncludeTenantPermissions(this SaasPermission permission) => new()
    {
        UserId = permission.UserId,
        TenantId = permission.TenantId,
        TenantPermissions = permission.TenantPermissions.Select(p => new TenantPermission
        {
            PermissionStr = p.PermissionStr,
            TenantId = permission.TenantId
        }).ToList()
    };
}
