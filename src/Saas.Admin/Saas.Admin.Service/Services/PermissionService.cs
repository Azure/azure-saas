namespace Saas.Admin.Service.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionServiceClient _permissionsServiceClient;

    public PermissionService(IPermissionServiceClient permissionServiceClient)
    {
        _permissionsServiceClient = permissionServiceClient;
    }

    public Task AddUserPermissionsToTenantAsyc(Guid tenantId, string userId, string[] permissions)
    {        
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Guid>> GetTenantsForUserAsync(string userId, string? filter)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>> GetTenantUsersAsync(Guid tenantId)
    {
        var response = await _permissionsServiceClient.GetTenantUsersAsync(tenantId.ToString());
        return response;
    }

    public Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(Guid tenantId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, string userId, string[] permissions)
    {
        throw new NotImplementedException();
    }
}
