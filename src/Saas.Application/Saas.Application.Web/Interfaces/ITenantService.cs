using Saas.Application.Web.Models;
namespace Saas.Application.Web.Interfaces
{
    public interface ITenantService
    {
        public Task<TenantViewModel> GetTenantByRouteAsync(string userIdentifier);
    }
}
