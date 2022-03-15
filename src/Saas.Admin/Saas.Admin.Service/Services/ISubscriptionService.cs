namespace Saas.Admin.Service.Services;
public interface ITenantService
{
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();

    Task<Tenant> GetTenantAsync(Guid tenantId);

    Task<Tenant> AddTenantAsync(Tenant tenant);

    Task<Tenant> UpdateTenantAsync(Tenant tenant);

    Task DeleteTenantAsync(Guid tenantId);

    Task<bool> TenantExistsAsync(Guid tenantId);
}
