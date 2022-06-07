﻿using Saas.Admin.Service.Controllers;
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

    public async Task<IEnumerable<TenantDTO>> GetAllTenantsAsync()
    {
        List<Tenant> allTenants = await _context.Tenants.ToListAsync();
        IEnumerable<TenantDTO> returnList = allTenants.Select(t => new TenantDTO(t));
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

    public async Task<IEnumerable<TenantDTO>> GetTenantsByIdAsync(IEnumerable<string> ids)
    {
        IQueryable<Tenant>? tenants = _context.Tenants.Where(t => ids.Contains(t.Id.ToString()));

        List<TenantDTO>? returnValue = await tenants.Select(t => new TenantDTO(t)).ToListAsync();
        return returnValue;
    }

    public async Task<TenantDTO> AddTenantAsync(NewTenantRequest newTenantRequest, string adminId)
    {
        Tenant tenant = newTenantRequest.ToTenant();
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        try
        {
            await _permissionService.AddUserPermissionsToTenantAsync(tenant.Id.ToString(), adminId, AppConstants.Roles.TenantAdmin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting permission for tenant {tenantName}", newTenantRequest.Name);
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            throw;
        }

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

    public async Task<TenantInfoDTO> GetTenantInfoByRouteAsync(string route)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(x => route != null && x.Route.Length == route.Length && EF.Functions.Like(x.Route, $"%{route}%"));
        TenantInfoDTO returnValue = new TenantInfoDTO(tenant);
        return returnValue;
    }

    public async Task<bool> CheckPathExists(string path)
    {
        try
        {
            bool exists = await _context.Tenants.AnyAsync(t => string.Equals(t.Route, path));
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking for valid path");
            throw;
        }
    }


}
