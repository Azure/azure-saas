namespace Saas.Admin.Service.Services;

public class TenantService : ITenantService
{
    private readonly TenantsContext _context;
    private readonly IPermissionService _permissionService;
    private readonly ILogger _logger;

    public TenantService(TenantsContext tenantContext, IPermissionService permissionService, ILogger<TenantService> logger)
    {
        _context = tenantContext;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        return await _context.Tenant.ToListAsync();
    }

    public async Task<Tenant> GetTenantAsync(Guid tenantId)
    {
        var tenant = await _context.Tenant.FindAsync(tenantId);

        if (tenant == null)
        {
            throw new ItemNotFoundExcepton("Tenant");
        }

        return tenant;
    }

    public async Task<Tenant> AddTenantAsync(Tenant tenant)
    {
        _context.Tenant.Add(tenant);
        await _context.SaveChangesAsync();

        return tenant;
    }

    public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        _context.Entry(tenant).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await TenantExistsAsync(tenant.Id))
            {
                throw new ItemNotFoundExcepton("Tenant");
            }
            else
            {
                throw;
            }
        }
        return tenant;
    }

    public async Task DeleteTenantAsync(Guid tenantId)
    {
        var tenant = await _context.Tenant.FindAsync(tenantId);
        if (tenant == null)
        {
            throw new ItemNotFoundExcepton("Tenant");
        }

        _context.Tenant.Remove(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId)
    {
        return await _context.Tenant.AnyAsync(e => e.Id == tenantId);
    }

    public async Task<IEnumerable<string>> GetTenantUsersAsync(Guid tenantId)
    {
        IEnumerable<string> users = await _permissionService.GetTenantUsersAsync(tenantId);
        return users;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(Guid tenantId, string userId)
    {
        IEnumerable<string> users = await _permissionService.GetUserPermissionsForTenantAsync(tenantId, userId);
        return users;
    }


    public async Task AddUserPermissionsToTenantAsync(Guid tenantId, string userId, string[] permissions)
    {
        await _permissionService.AddUserPermissionsToTenantAsyc(tenantId, userId, permissions);
    }

    public async Task RemoveUserPermissionsFromTenantAsync(Guid tenantId, string userId, string[] permissions)
    {
        await _permissionService.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
    }

    public async Task<IEnumerable<Guid>> GetTenantsForUserAsync(string userId, string? filter = null)
    {
        IEnumerable<Guid> tenants = await _permissionService.GetTenantsForUserAsync(userId, filter);
        return tenants;
    }
}
