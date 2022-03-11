using Microsoft.AspNetCore.Mvc;

namespace Saas.SignupAdministration.Web.Controllers
{
    [Route(SR.TenantRoute)]
    public class RequestPathController : Controller
    {
        [Route(SR.TenantRoute)]
        [HttpGet]
        public IActionResult Index(string tenant)
        {
            ViewBag.Tenant = tenant;

            return View();
        }
    }
}
