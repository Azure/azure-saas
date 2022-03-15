namespace Saas.Admin.Service.Services;

public class PermissionService : IPermissionService
{
    public Task AddUserPermissionsToTenantAsyc(Guid tenantId, string userId, string[] permissions)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Guid>> GetTenantsForUserAsync(string userId, string? filter)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetTenantUsersAsync(Guid tenantId)
    {
        throw new NotImplementedException();
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
