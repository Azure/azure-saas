using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saas.Admin.Web.Models;
using Saas.Admin.Web.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Saas.Admin.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITenantService _tenantService;

        public HomeController(ILogger<HomeController> logger, ITenantService tenantService)
        {
            _logger = logger;
            _tenantService = tenantService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            return View(await _tenantService.GetItemsAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
