using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Services
{
    public class TenantService : ITenantService
    {
        private HttpClient _client;
        private IAdminServiceClient _adminServiceClient;
        private IOptions<AppSettings> _appSettings;
        public TenantService(HttpClient client, IAdminServiceClient adminServiceClient, IOptions<AppSettings> appSettings)
        {
            _client = client;
            _adminServiceClient = adminServiceClient;
            _appSettings = appSettings;
        }

        public async Task<TenantViewModel> GetTenantByRouteAsync(string userIdentifer)
        {
            if (userIdentifer != null)
            {
                var tenants = await _adminServiceClient.TenantsAsync(userIdentifer, _appSettings.Value.AppTenantId.Substring(0, AppConstants.Tenant.IdLength));
                var activeTenant = tenants.FirstOrDefault();

                if (activeTenant != null)
                    return new TenantViewModel() { Id = activeTenant.Id, Name = activeTenant.Name };
            }

            return null;
        }
    }
}
