using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    public class AdminController : Controller
    {
        [Route("{tenant}/admin/dashboard")]
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Route("{tenant}/admin/manage")]
        [HttpGet]
        public IActionResult Manage()
        {
            return View();
        }

        [Route("{tenant}/admin/users")]
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }
    }
}
