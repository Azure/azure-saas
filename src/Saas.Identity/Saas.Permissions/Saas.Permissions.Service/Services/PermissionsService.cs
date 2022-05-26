using Saas.Permissions.Service.Data;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Services;

public class PermissionsService : IPermissionsService
{
    private readonly PermissionsContext _context;
    private readonly ILogger _logger;
    private readonly IGraphAPIService _graphAPIService;
    public PermissionsService(PermissionsContext permissionsContext, ILogger<PermissionsService> logger, IGraphAPIService graphAPIService)
    {
        _context = permissionsContext;
        _logger = logger;
        _graphAPIService = graphAPIService;
    }

    public async Task<ICollection<Permission>> GetPermissionsAsync(string userId)
    {
        _logger.LogDebug("User {userId} tried to get permissions", userId);
        return await _context.Permissions
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<ICollection<string>> GetTenantUsersAsync(string tenantId)
    {
        _logger.LogDebug("Users are requested from {tenantId}", tenantId);
        return await _context.Permissions
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.UserId)
            .ToListAsync();
    }

    public async Task<ICollection<string>> GetUserPermissionsForTenantAsync(string tenantId, string userId)
    {
        _logger.LogDebug("User permissions where requested for {userId} for {tenantId}", userId, tenantId);
        return await _context.Permissions
            .Where(x => x.UserId == userId && x.TenantId == tenantId)
            .Select(x => x.ToTenantPermissionString())
            .ToListAsync();
    }

    public async Task AddUserPermissionsToTenantAsync(string tenantId, string userId, string[] permissions)
    {
        _logger.LogDebug("User permissions where requested to be added to {userId} on {tenantId}", userId, tenantId);
        foreach (var permission in permissions)
        {
            if (await GetPermissionExistsAsync(tenantId, userId, permission))
            {
                throw new ItemAlreadyExistsException($"User: {userId} has already been granted {permission} on tenant: {tenantId}");
            }

            _context.Permissions.Add(new Permission { TenantId = tenantId, UserId = userId, PermissionStr = permission });
        }
        await _context.SaveChangesAsync();
        return;
    }

    public async Task AddUserPermissionsToTenantByEmailAsync(string tenantId, string userEmail, string[] permissions)
    {
        _logger.LogDebug("User permissions where requested to be added to {userEmail} on {tenantId}", userEmail, tenantId);
        User user = await _graphAPIService.GetUserByEmail(userEmail);
        foreach (var permission in permissions)
        {
            if (await GetPermissionExistsAsync(tenantId, user.UserId, permission))
            {
                throw new ItemAlreadyExistsException($"User: {user.UserId} has already been granted {permission} on tenant: {tenantId}");
            }

            _context.Permissions.Add(new Permission { TenantId = tenantId, UserId = user.UserId, PermissionStr = permission });
        }
        await _context.SaveChangesAsync();
        return;
    }

    public async Task RemoveUserPermissionsFromTenantAsync(string tenantId, string userId, string[] permissions)
    {
        _logger.LogDebug("Permissions were requested to be removed for {userId} on {tenantId}", userId, tenantId);
        foreach (var permission in permissions)
        {
            Permission? dbPermission = await _context.Permissions.FirstOrDefaultAsync(x => 
            x.TenantId == tenantId && 
            x.UserId == userId && 
            x.PermissionStr == permission);

            if (dbPermission == null) {
                throw new ItemNotFoundExcepton($"Permission {permission} not found for User {userId} on Tenant {tenantId}");
            }

            _context.Permissions.Remove(dbPermission);
        }
        await _context.SaveChangesAsync();

    }

    /// <summary>
    /// Optionally supply a filter for a specific TenantId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public async Task<ICollection<string>> GetTenantsForUserAsync(string userId, string? filter)
    {
        _logger.LogDebug("{userId} has requested tenants", userId);
        return await _context.Permissions
            .Where(x => x.UserId == userId)
            .Where(x => filter == null || x.TenantId.Length == filter.Length && EF.Functions.Like(x.TenantId, $"%{filter}%"))
            .Select(x => x.TenantId)
            .ToListAsync();
    }

    private async Task<bool> GetPermissionExistsAsync(string tenantId, string userId, string permission)
    {
        _logger.LogDebug("{userId} is checking if {permission} exists on {tenantId}", userId, permission, tenantId);
        return await _context.Permissions.AnyAsync(x =>
        x.UserId == userId &&
        x.TenantId == tenantId &&
        x.PermissionStr == permission);
    }

}
