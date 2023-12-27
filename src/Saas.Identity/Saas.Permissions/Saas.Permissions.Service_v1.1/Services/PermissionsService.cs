using Saas.Identity.Authorization.Model.Data;
using Saas.Identity.Authorization.Model.Kind;
using Saas.Permissions.Service.Data.Context;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Services;

public class PermissionsService(
    SaasPermissionsContext permissionsContext,
    ILogger<PermissionsService> logger,
    IGraphAPIService graphAPIService) : IPermissionsService
{
    private readonly SaasPermissionsContext _permissionsContext = permissionsContext;
    private readonly ILogger _logger = logger;
    private readonly IGraphAPIService _graphAPIService = graphAPIService;

    public async Task<ICollection<SaasPermission>> GetPermissionsAsync(Guid userId)
    {
        _logger.LogDebug("User {userId} tried to get permissions", userId);

        return await _permissionsContext.SaasPermissions
            .Include(x => x.UserPermissions)
            .Include(x => x.TenantPermissions)
            .Where(x => x.UserId == userId)
            .Select(x => x.IncludeAllPermissionSets())
            .ToListAsync();
    }

    public async Task<ICollection<Guid>> GetTenantUsersAsync(Guid tenantId)
    {
        _logger.LogDebug("Users are requested from {tenantId}", tenantId);


        return await _permissionsContext.SaasPermissions
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.UserId)
            .ToListAsync();
    }

    public async Task<ICollection<string>> GetUserPermissionClaimsForTenantAsync(Guid tenantId, Guid userId)
    {
        _logger.LogDebug("User permissions where requested for {userId} for {tenantId}", userId, tenantId);

        var permission =  await _permissionsContext.SaasPermissions
            .Include(x => x.UserPermissions)
            .Include(x => x.TenantPermissions)
            .Where(x => x.UserId == userId && x.TenantId == tenantId)
            .Select(x => x.IncludeAllPermissionSets())
            .ToListAsync();

        return permission.SelectMany(x => x.TenantPermissions.Select(t => t.ToClaim())
                .Concat(x.UserPermissions.Select(u => u.ToClaim()))).ToList();
    }

    public async Task AddNewTenantAsync(Guid tenantId, Guid userId)
    {
        _logger.LogDebug("New Tenant creation was requested by {userId} for {tenantId}", userId, tenantId);

        TenantPermission newTenantPermissions = new()
        {
            PermissionStr = TenantPermissionKind.Admin.ToString(),
        };

        UserPermission newUserPermission = new()
        {
            PermissionStr = UserPermissionKind.Self.ToString(),
        };

        _permissionsContext.SaasPermissions.Add(new SaasPermission
        {
            TenantId = tenantId,
            UserId = userId,
            TenantPermissions = new TenantPermission[] { newTenantPermissions },
            UserPermissions = new UserPermission[] { newUserPermission }
        });

        await _permissionsContext.SaveChangesAsync();

        return;
    }

    public async Task AddUserPermissionsToUserAsync(Guid tenantId, Guid userId, string[] permissions)
    {
        _logger.LogDebug($"User permissions where requested to be added to user with id '{userId}' on tenant with id '{tenantId}'");

        _permissionsContext.SaasPermissions.Add(new SaasPermission
        {
            TenantId = tenantId,
            UserId = userId,
            UserPermissions = permissions.Select(permission => new UserPermission { PermissionStr = permission }).ToList(),
        });

        await _permissionsContext.SaveChangesAsync();
    }

    public async Task AddUserPermissionsToTenantAsync(Guid tenantId, Guid userId, string[] permissions)
    {
        _logger.LogDebug("User permissions where requested to be added to {userId} on {tenantId}", userId, tenantId);

        _permissionsContext.SaasPermissions.Add(new SaasPermission
        {
            TenantId = tenantId,
            UserId = userId,
            TenantPermissions = permissions.Select(permission => new TenantPermission { PermissionStr = permission }).ToList()
        });

        await _permissionsContext.SaveChangesAsync();

        return;
    }

    public async Task AddUserPermissionsToTenantByEmailAsync(Guid tenantId, string userEmail, string[] permissions)
    {
        _logger.LogDebug($"User permissions where requested to be added to {userEmail} on {tenantId}");
        
        User user = await _graphAPIService.GetUserByEmail(userEmail);

        if (user?.UserId is null)
        {
            throw new ItemNotFoundException($"User with email: {userEmail} was not found");
        }

        if (!Guid.TryParse(user?.UserId, out Guid userIdGuid))
        {
            throw new InvalidOperationException($"User id '{user?.UserId}' is not a guid.");
        }
                
        _permissionsContext.SaasPermissions
            .Update(new SaasPermission
            {
                TenantId = tenantId,
                UserId = userIdGuid,
                TenantPermissions = permissions.Select(permission => new TenantPermission { PermissionStr = permission }).ToArray()
            });

        await _permissionsContext.SaveChangesAsync();
    }

    public async Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, Guid userId, string[] permissions)
    {
        _logger.LogDebug("Permissions were requested to be removed for {userId} on {tenantId}", userId, tenantId);

        var removeTenantPermission = await _permissionsContext.SaasPermissions
            .Include(x => x.TenantPermissions)
            .Where(x => x.TenantId == tenantId && x.UserId == userId)
            .Select(x => x.IncludeTenantPermissions())
            .SelectMany(x => x.TenantPermissions.Where(x => permissions.Contains(x.PermissionStr)))
            .ToListAsync();

        _permissionsContext.SaasPermissions
            .RemoveRange(removeTenantPermission
                .Select(x => new SaasPermission 
                { 
                    TenantId = tenantId,
                    UserId = userId,
                    TenantPermissions = new[] { x } 
                }));

        await _permissionsContext.SaveChangesAsync();
    }

    public async Task<ICollection<Guid>> GetTenantsForUserAsync(Guid userId)
    {
        _logger.LogDebug("{userId} has requested tenants", userId);

        return await _permissionsContext.SaasPermissions
            .Where(x => x.UserId == userId)
            .Select(x => x.TenantId)
            .ToListAsync();
    }
}
