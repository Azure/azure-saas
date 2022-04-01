using Saas.Permissions.Api.Data;
using Saas.Permissions.Api.Exceptions;
using Saas.Permissions.Api.Interfaces;

namespace Saas.Permissions.Api.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly PermissionsContext _context;
        public PermissionsService(PermissionsContext permissionsContext)
        {
            _context = permissionsContext;
        }

        public async Task<ICollection<Permission>> GetPermissionsAsync(Guid userId)
        {
            return await _context.Permissions
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Guid>> GetTenantUsersAsync(Guid tenantId)
        {
            return await _context.Permissions
                .Where(x => x.TenantId == tenantId)
                .Select(x => x.TenantId)
                .ToListAsync();
        }

        public async Task<ICollection<string>> GetUserPermissionsForTenantAsync(Guid tenantId, Guid userId)
        {
            return await _context.Permissions
                .Where(x => x.UserId == userId && x.TenantId == tenantId)
                .Select(x => x.ToTenantPermissionString())
                .ToListAsync();
        }

        public async Task AddUserPermissionsToTenantAsync(Guid tenantId, Guid userId, string[] permissions)
        {
            foreach (var permission in permissions)
            {
                _context.Permissions.Add(new Permission { TenantId = tenantId, UserId = userId, PermissionStr = permission });
            }
            await _context.SaveChangesAsync();
            return;
        }

        public async Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, Guid userId, string[] permissions)
        {
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

        // filter not currently implemented.
        public async Task<ICollection<Guid>> GetTenantsForUserAsync(Guid userId, string? filter)
        {
            return await _context.Permissions
                .Where(x => x.UserId == userId)
                .Select(x => x.TenantId)
                .ToListAsync();
        }

    }
}
