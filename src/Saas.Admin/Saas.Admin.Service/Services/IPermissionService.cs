using Saas.Admin.Service.Controllers;

namespace Saas.Admin.Service.Services;

public interface IPermissionService
{
    Task<IEnumerable<UserDTO>> GetTenantUsersAsync(string tenantId);
    Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId);
    Task AddUserPermissionsToTenantAsync(string tenantId, string userId, params string[] permissions);
    Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, params string[] permissions);
    Task<IEnumerable<string>> GetTenantsForUserAsync(string userId, string? filter);
    Task AddUserPermissionsToTenantByEmailAsync(string tenantId, string userEmail, params string[] permissions);
}
