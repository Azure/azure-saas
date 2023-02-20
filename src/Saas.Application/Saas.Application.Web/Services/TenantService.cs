using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Services;

public class TenantService : ITenantService
{
    private readonly IAdminServiceClient _adminServiceClient;
    public TenantService(IAdminServiceClient adminServiceClient)
    {
        _adminServiceClient = adminServiceClient;
    }

    // TODO (SaaS): Define the necessary public tenant information necessary for non-member users

    public async Task<TenantViewModel> GetTenantInfoByRouteAsync(string route)
    {
        TenantInfoDTO? tenant = null;

        await _adminServiceClient.IsValidPathAsync(route);

        if (route is not null)
            tenant = await _adminServiceClient.TenantinfoAsync(route);

        return new TenantViewModel() 
        { 
            Id = tenant?.Id ?? Guid.Empty, 
            Name = tenant?.Name ?? "Unknown" 
        };
    }
}
