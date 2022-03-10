using DemoApplication.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace DemoApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize(Roles = "SuperAdmin")]
        [Route("subscriptions/{subscriptionId}/SuperAdmins")]
        public IActionResult GetUsersSuperAdmin(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Roles = "SystemAdmin, SubscriptionAdmin")]
        [Route("subscriptions/{subscriptionId}/Admins")]
        public IActionResult GetUsersAdmin(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Roles = "SubscriptionAdmin")]
        [Route("subscriptions/{subscriptionId}/SubAdmins")]
        public IActionResult GetUsersSubAdmin(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Roles = "SubscriptionUser")]
        [Route("subscriptions/{subscriptionId}/SubUsers")]
        public IActionResult GetUsersSubUser(string customerId)
        {
            return RedirectToAction("privacy");
        }


        [Authorize(Policy = "SuperAdminOnly")]
        [Route("subscriptions/{subscriptionId}/superAdminPolicy")]
        public IActionResult GetUsersSuperAdminPolicy(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Policy = "AdminsOnlyPolicy")]
        [Route("subscriptions/{subscriptionId}/AdminsPolicy")]
        public IActionResult GetUsersAdminPolicy(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Policy = "SubscriptionAdminOnly")]
        [Route("subscriptions/{subscriptionId}/SubAdminsPolicy")]
        public IActionResult GetUsersSubAdminPolicy(string customerId)
        {
            return RedirectToAction("privacy");
        }

        [Authorize(Policy = "SubscriptionUsersOnly")]
        [Route("subscriptions/{subscriptionId}/SubUsersPolicy")]
        public IActionResult GetUsersSubUserPolicy(string customerId)
        {
            return RedirectToAction("privacy");
        }
    }
}