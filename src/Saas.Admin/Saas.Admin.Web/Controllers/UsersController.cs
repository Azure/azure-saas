using Microsoft.AspNetCore.Mvc;

namespace Saas.Admin.Web.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
