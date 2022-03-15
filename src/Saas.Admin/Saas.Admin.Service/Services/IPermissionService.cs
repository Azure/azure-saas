namespace Saas.Admin.Service.Services;

public interface IPermissionService
{
    Task<IEnumerable<string>> GetTenantUsersAsync(Guid tenantId);
    Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(Guid tenantId, string userId);
    Task AddUserPermissionsToTenantAsyc(Guid tenantId, string userId, string[] permissions);
    Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, string userId, string[] permissions);
    Task<IEnumerable<Guid>> GetTenantsForUserAsync(string userId, string? filter);
}
