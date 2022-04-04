namespace Saas.Permissions.Api.Data;

public class Permission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string PermissionStr { get; set; } = string.Empty;

    internal string ToTenantPermissionString()
    {
        return $"{TenantId}.{PermissionStr}";
    }

}
