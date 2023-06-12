using Saas.Admin.Service.Controllers;

namespace Saas.Admin.Service.Services;

public interface ITenantService
{
    Task<IEnumerable<TenantDTO>> GetAllTenantsAsync();

    Task<TenantDTO> GetTenantAsync(Guid tenantId);
    Task<IEnumerable<TenantDTO>> GetTenantsByIdAsync(IEnumerable<Guid> ids);

    Task<TenantDTO> AddTenantAsync(NewTenantRequest newTenantRequest, Guid adminId);

    Task<TenantDTO> AddUserToTenantAsync(NewTenantRequest newTenantRequest, Guid adminId);
    

    Task<TenantDTO> UpdateTenantAsync(TenantDTO tenant);

    Task DeleteTenantAsync(Guid tenantId);

    Task<TenantInfoDTO> GetTenantInfoByRouteAsync(string route);
    Task<bool> TenantExistsAsync(Guid tenantId);
    Task<bool> CheckPathExists(string path);
}
