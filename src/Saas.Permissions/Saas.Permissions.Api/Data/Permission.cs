namespace Saas.Permissions.Api.Data
{
    public class Permission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public string PermissionStr { get; set; } = string.Empty;

        internal string ToTenantPermissionString()
        {
            return $"{TenantId}.{PermissionStr}";
        }

    }
}
