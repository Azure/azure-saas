using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Saas.Application.Web.Interfaces;
using Saas.Application.Web.Models;

namespace Saas.Application.Web.Pages
{
    public class TenantModel : PageModel
    {
        private readonly ITenantService _tenantService;
        public Tenant? tenantData;
        public TenantModel(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IActionResult> OnGetAsync(string tenantRoute)
        {
            if (String.IsNullOrWhiteSpace(tenantRoute))
            {
                return NotFound();
            }

            tenantData = await _tenantService.GetTenantByRouteAsync(tenantRoute);
            return Page();

        }
    }
}
