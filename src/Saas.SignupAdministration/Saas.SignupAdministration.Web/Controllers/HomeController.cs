using System.Diagnostics;

namespace Saas.SignupAdministration.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmail _email; 

        public HomeController(ILogger<HomeController> logger, IEmail email)
        {
            _logger = logger;
            _email = email; 
        }

        [HttpGet]
        public IActionResult Help()
        {
            _email.Send("jason.berg@thespurgroup.com"); 
            return View();
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pricing()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
