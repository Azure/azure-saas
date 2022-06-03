using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
    private readonly ITenantService _tenantService;
    private readonly IApplicationUser _applicationUser;
    public TenantViewModel? tenantData;
    private string activeRoute = string.Empty;

    public bool DisplayTenantInfo => _applicationUser != null && !string.IsNullOrWhiteSpace(activeRoute);

    public IndexModel(ILogger<IndexModel> logger, ITenantService tenantService, IApplicationUser applicationUser)
    {
        _logger = logger;
        _tenantService = tenantService;
        _applicationUser = applicationUser;
    }

    public async Task<IActionResult> OnGetAsync(string? route)
    {
        activeRoute = route ?? string.Empty;

        if (DisplayTenantInfo)
        {
            tenantData = await _tenantService.GetTenantInfoByRouteAsync(activeRoute);
        }

        return Page();
    }
}