using Saas.Identity.Authorization.Model.Data;

namespace Saas.Permissions.Service.Interfaces;

public interface IPermissionsService
{
    Task<ICollection<SaasPermission>> GetPermissionsAsync(Guid userId);
    Task<ICollection<Guid>> GetTenantUsersAsync(Guid tenantId);
    Task<ICollection<string>> GetUserPermissionClaimsForTenantAsync(Guid tenantId, Guid userId);
    Task AddNewTenantAsync(Guid tenantId, Guid userId);
    Task AddUserPermissionsToTenantAsync(Guid tenantId, Guid userId, string[] permissions);
    Task AddUserPermissionsToUserAsync(Guid tenantId, Guid userId, string[] permissions);
    Task AddUserPermissionsToTenantByEmailAsync(Guid tenantId, string userEmail, string[] permissions);
    Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, Guid userId, string[] permissions);
    Task<ICollection<Guid>> GetTenantsForUserAsync(Guid userId);    
}
