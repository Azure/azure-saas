﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web.Pages;

public class IndexModel : PageModel
{
    // TODO (SaaS): Connect your main page service calls here to begin guiding users both authenticated and otherwise along the right path

    private readonly ILogger<IndexModel> _logger;
    private readonly ITenantService _tenantService;
    private readonly IApplicationUser _applicationUser;
    public TenantViewModel? tenantData;
    private string activeRoute = string.Empty;

    public bool DisplayTenantInfo => _applicationUser is not null && !string.IsNullOrWhiteSpace(activeRoute);

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
            try
            {
                tenantData = await _tenantService.GetTenantInfoByRouteAsync(activeRoute);
            }
            catch
            {

            }            
        }

        return Page();
    }
}