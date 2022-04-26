using Saas.Permissions.Service.Data;

namespace Saas.Permissions.Service.Interfaces;

public interface IPermissionsService
{
    public Task<ICollection<Permission>> GetPermissionsAsync(string userId);
    public Task<ICollection<string>> GetTenantUsersAsync(string tenantId);
    public Task<ICollection<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId);
    public Task AddUserPermissionsToTenantAsync(string tenantId, string userId, string[] permissions);
    public Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, string[] permissions);
    public Task<ICollection<string>> GetTenantsForUserAsync(string userId, string? filter);
}
