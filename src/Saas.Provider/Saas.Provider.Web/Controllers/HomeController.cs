using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saas.Provider.Web.Models;
using Saas.Provider.Web.Models.CosmosDb;
using Saas.Provider.Web.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICosmosDbService _cosmosDbService;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _userManager = userManager;
            _cosmosDbService = cosmosDbService;
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync(string emailAddress)
        {
            ViewBag.EmailAddress = emailAddress;

            // Do a check to see if username already taken
            var user = new ApplicationUser { UserName = emailAddress, Email = emailAddress };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Create order process id and object
                Item item = new Item()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Onboarding Flow",
                    UserId = user.Id,
                    IsExistingUser = "false",
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Created = DateTime.Now
                };

                // Commit to CosmosDB
                try
                {
                    await _cosmosDbService.AddItemAsync(item);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                }

                return RedirectToAction("name", "create", new { id = item.Id, userId = user.Id, isExistingUser = "false" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay view
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

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
