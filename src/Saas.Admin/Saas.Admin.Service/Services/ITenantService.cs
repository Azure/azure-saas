using Saas.Admin.Service.Controllers;

namespace Saas.Admin.Service.Services;

public interface ITenantService
{
    Task<IList<TenantDTO>> GetAllTenantsAsync();

    Task<TenantDTO> GetTenantAsync(Guid tenantId);

    Task<TenantDTO> AddTenantAsync(NewTenantRequest newTenantRequest);

    Task<TenantDTO> UpdateTenantAsync(TenantDTO tenant);

    Task DeleteTenantAsync(Guid tenantId);

    Task<bool> TenantExistsAsync(Guid tenantId);
}
