using Saas.Application.Web.Interfaces;
using Saas.Application.Web.Models;

namespace Saas.Application.Web.Services
{
    public class TenantService : ITenantService
    {
        private HttpClient _client;
        public TenantService(HttpClient client)
        {
            _client = client;
        }

        public async Task<Tenant> GetTenantByRouteAsync(string routeName)
        {
            // TODO: Implement admin api and test this route
            //return await _client.GetFromJsonAsync<Tenant>($"/tenants?routeName={routeName}"); 
            return new Tenant { Id = 1, Name = routeName };
        }
    }
}
