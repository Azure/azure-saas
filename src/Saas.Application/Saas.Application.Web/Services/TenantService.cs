using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Services
{
    public class TenantService : ITenantService
    {
        private HttpClient _client;
        private IAdminServiceClient _adminServiceClient;
        public TenantService(HttpClient client, IAdminServiceClient adminServiceClient)
        {
            _client = client;
            _adminServiceClient = adminServiceClient;
        }

        public async Task<TenantViewModel> GetTenantByRouteAsync(string routeName)
        {
            Guid guid = new Guid();
            if (Guid.TryParse(routeName, out guid))
            {
                var tenant = await _adminServiceClient.TenantsGETAsync(guid);
                return new TenantViewModel() { Id = tenant.Id, Name = tenant.Name };
            }

            return null;
        }
    }
}
