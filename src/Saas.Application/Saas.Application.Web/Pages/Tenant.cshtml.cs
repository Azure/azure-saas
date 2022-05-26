using Microsoft.AspNetCore.Mvc.RazorPages;
using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Pages
{
    [Authorize]
    public class TenantModel : PageModel
    {
        private readonly ITenantService _tenantService;
        private readonly IApplicationUser _applicationUser;

        public TenantViewModel? tenantData;
        public TenantModel(ITenantService tenantService, IApplicationUser applicationUser)
        {
            _tenantService = tenantService;
            _applicationUser = applicationUser;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_applicationUser == null)
                return Redirect("Index");

            tenantData = await _tenantService.GetTenantByRouteAsync(_applicationUser.NameIdentifier.ToString());
            return Page();

        }
    }
}
