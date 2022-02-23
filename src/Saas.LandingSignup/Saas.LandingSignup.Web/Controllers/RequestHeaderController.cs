using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    public class RequestHeaderController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
