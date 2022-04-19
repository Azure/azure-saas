namespace Saas.Admin.Service.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionServiceClient _permissionsServiceClient;

    public PermissionService(IPermissionServiceClient permissionServiceClient)
    {
        _permissionsServiceClient = permissionServiceClient;
    }

    public async Task AddUserPermissionsToTenantAsync(string tenantId, string userId, string[] permissions)
    {
        await _permissionsServiceClient.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
        return;
    }

    public async Task<IEnumerable<string>> GetTenantsForUserAsync(string userId, string? filter)
    {
        return await _permissionsServiceClient.GetTenantsForUserAsync(userId, filter);
    }

    public async Task<IEnumerable<string>> GetTenantUsersAsync(string tenantId)
    {
        return await _permissionsServiceClient.GetTenantUsersAsync(tenantId);
    }

    public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId)
    {
        return await _permissionsServiceClient.GetUserPermissionsForTenantAsync(tenantId, userId);
    }

    public async Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, string[] permissions)
    {
        await _permissionsServiceClient.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
        return;
    }
}
