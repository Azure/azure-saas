using Saas.Admin.Service.Controllers;
using Saas.Admin.Service.Data;

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

    public async Task<IList<TenantDTO>> GetAllTenantsAsync()
    {
        List<Tenant> allTenants = await _context.Tenants.ToListAsync();
        List<TenantDTO> returnList = allTenants.Select(t => new TenantDTO(t)).ToList();
        return returnList;
    }

    public async Task<TenantDTO> GetTenantAsync(Guid tenantId)
    {
        Tenant? tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            throw new ItemNotFoundExcepton("Tenant");
        }

        TenantDTO returnValue = new TenantDTO(tenant);
        return returnValue;
    }

    public async Task<TenantDTO> AddTenantAsync(NewTenantRequest newTenantRequest)
    {
        Tenant tenant = newTenantRequest.ToTenant();
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        //TODO: Add permissiosn for the user

        TenantDTO? returnValue = new TenantDTO(tenant);
        return returnValue;
    }

    public async Task<TenantDTO> UpdateTenantAsync(TenantDTO tenantDto)
    {
        Tenant? fromDB = await _context.Tenants.FindAsync(tenantDto.Id);

        if (fromDB == null)
        {
            throw new ItemNotFoundExcepton("Tenant");
        }

        tenantDto.CopyTo(fromDB);
        _context.Entry(fromDB).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogInformation(ex, "Concurrency exception saving changes to {TenatnID}, {TenantName}", fromDB.Id, fromDB.Name);
            throw;
        }

        TenantDTO returnValue = new TenantDTO(fromDB);
        return returnValue;
    }

    public async Task DeleteTenantAsync(Guid tenantId)
    {
        Tenant? tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            throw new ItemNotFoundExcepton("Tenant");
        }

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId)
    {
        return await _context.Tenants.AnyAsync(e => e.Id == tenantId);
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
