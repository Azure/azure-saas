using Saas.Admin.Service.Controllers;

namespace Saas.Admin.Service.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionServiceClient _permissionsServiceClient;

    public PermissionService(IPermissionServiceClient permissionServiceClient)
    {
        _permissionsServiceClient = permissionServiceClient;
    }

    public async Task AddUserPermissionsToTenantAsync(string tenantId, string userId, params string[] permissions)
    {
        await _permissionsServiceClient.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
        return;
    }

    public async Task<IEnumerable<string>> GetTenantsForUserAsync(string userId, string? filter)
    {
        return await _permissionsServiceClient.GetTenantsForUserAsync(userId, filter);
    }

    public async Task<IEnumerable<UserDTO>> GetTenantUsersAsync(string tenantId)
    {
        ICollection<User>? users = await _permissionsServiceClient.GetTenantUsersAsync(tenantId);

        IEnumerable<UserDTO>? retVal = users.Select(u => new UserDTO(u.UserId, u.DisplayName));
        return retVal;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId)
    {
        return await _permissionsServiceClient.GetUserPermissionsForTenantAsync(tenantId, userId);
    }

    public async Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, params string[] permissions)
    {
        await _permissionsServiceClient.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
        return;
    }
}
