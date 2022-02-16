using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    [Route("/{tenant}")]
    public class RequestPathController : Controller
    {
        [Route("/{tenant}")]
        public IActionResult Index(string tenant)
        {
            ViewBag.Tenant = tenant;

            return View();
        }
    }
}
