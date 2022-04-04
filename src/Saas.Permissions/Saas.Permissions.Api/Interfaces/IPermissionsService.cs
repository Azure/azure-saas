using Saas.Permissions.Api.Data;

namespace Saas.Permissions.Api.Interfaces;

public interface IPermissionsService
{
    public Task<ICollection<Permission>> GetPermissionsAsync(string userId);
    public Task<ICollection<Guid>> GetTenantUsersAsync(Guid tenantId);
    public Task<ICollection<string>> GetUserPermissionsForTenantAsync(Guid tenantId, string userId);
    public Task AddUserPermissionsToTenantAsync(Guid tenantId, string userId, string[] permissions);
    public Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, string userId, string[] permissions);
    public Task<ICollection<Guid>> GetTenantsForUserAsync(string userId, string? filter);
}
