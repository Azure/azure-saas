using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    public class RequestHeaderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
