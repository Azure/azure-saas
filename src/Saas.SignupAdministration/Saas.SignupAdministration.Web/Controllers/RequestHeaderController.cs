using Microsoft.AspNetCore.Mvc;

namespace Saas.SignupAdministration.Web.Controllers
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
