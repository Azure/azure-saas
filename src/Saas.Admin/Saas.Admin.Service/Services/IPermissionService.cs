namespace Saas.Admin.Service.Services;

public interface IPermissionService
{
    Task<IEnumerable<string>> GetTenantUsersAsync(string tenantId);
    Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId);
    Task AddUserPermissionsToTenantAsync(string tenantId, string userId, string[] permissions);
    Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, string[] permissions);
    Task<IEnumerable<string>> GetTenantsForUserAsync(string userId, string? filter);
}
