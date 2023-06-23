
namespace Saas.Identity.Services;
public interface ICustomTenantService
{
    public const int GetActiveTenantFlag = 1;
    public const int UpdateActiveTenantFlag = 2;

    Task<Guid> GetActiveTenantAsync(Guid userId);

    Task<bool> UpdateActiveTenantAsync(Guid userId, Guid tenantId);
}
