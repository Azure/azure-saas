using Microsoft.AspNetCore.Mvc;

namespace Saas.LandingSignup.Web.Controllers
{
    public class AdminController : Controller
    {
        [Route("{tenant}/admin/dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Route("{tenant}/admin/manage")]
        public IActionResult Manage()
        {
            return View();
        }

        [Route("{tenant}/admin/users")]
        public IActionResult Users()
        {
            return View();
        }
    }
}
