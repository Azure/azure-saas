using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    [Route("/" + SR.TenantTemplate)]
    public class RequestPathController : Controller
    {
        [Route("/" + SR.TenantTemplate)]
        [HttpGet]
        public IActionResult Index(string tenant)
        {
            ViewBag.Tenant = tenant;

            return View();
        }
    }
}
