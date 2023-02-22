using System.Runtime.CompilerServices;

namespace Saas.Permissions.Service.Data;

public class Permission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string PermissionStr { get; set; } = string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal string ToTenantPermissionString()
    {
        return $"Tenant.{TenantId}.{PermissionStr}";
    }

}
