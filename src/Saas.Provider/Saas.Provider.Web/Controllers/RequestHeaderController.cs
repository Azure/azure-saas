using Microsoft.AspNetCore.Mvc;

namespace Saas.Provider.Web.Controllers
{
    public class RequestHeaderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
