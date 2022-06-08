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

        // TODO (SaaS): Define the necessary public tenant information necessary for non-member users

        public async Task<TenantViewModel> GetTenantInfoByRouteAsync(string route)
        {
            TenantInfoDTO? tenant = null;

            await _adminServiceClient.IsValidPathAsync(route);

            if (route != null)
                tenant = await _adminServiceClient.TenantsGET2Async(route);

            return new TenantViewModel() { Id = tenant?.Id ?? Guid.Empty, Name = tenant?.Name ?? "Unknown" };
        }
    }
}
