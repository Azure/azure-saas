using Saas.Permissions.Api.Data;

namespace Saas.Permissions.Api.Interfaces
{
    public interface IPermissionsService
    {
        public Task<ICollection<Permission>> GetPermissionsAsync(Guid userId);
        public Task<ICollection<Guid>> GetTenantUsersAsync(Guid tenantId);
        public Task<ICollection<string>> GetUserPermissionsForTenantAsync(Guid tenantId, Guid userId);
        public Task AddUserPermissionsToTenantAsync(Guid tenantId, Guid userId, string[] permissions);
        public Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, Guid userId, string[] permissions);
        public Task<ICollection<Guid>> GetTenantsForUserAsync(Guid userId, string? filter);
    }
}
