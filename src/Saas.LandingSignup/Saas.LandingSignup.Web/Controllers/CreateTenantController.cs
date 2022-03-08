using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Controllers
{
    public class CreateTenantController : Controller
    {
        private readonly ILogger<CreateTenantController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateTenantController(ILogger<CreateTenantController> logger, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Step 4 - Submitted Tenant Creation
        [Route(SR.CreateTenantDeployRoute)]
        [HttpGet]
        public async Task<IActionResult> DeployTenantAsync(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            var httpClient = new HttpClient();
            var onboardingClient = new OnboardingClient(_appSettings.OnboardingApiBaseUrl, httpClient);

            Services.Tenant tenant = new Services.Tenant()
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsActive = true,
                IsCancelled = false,
                IsProvisioned = true,
                ApiKey = Guid.NewGuid(),
                CategoryId = categoryId,
                ProductId = productId,
                UserId = userId,
            };

            await onboardingClient.TenantsPOSTAsync(tenant);

            // Recreate order process id and object and set IsComplete = true
            var onboardingWorkflowItem = new OnboardingWorkflowItem()
            {
                Id = id,
                OnboardingWorkflowName = SR.OnboardingWorkflowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                ProductId = productId,
                IsComplete = true,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            ViewBag.OnboardingWorkflowItem = onboardingWorkflowItem;

            return RedirectToAction(SR.ConfirmationAction, SR.CreateTenantController, new { isExistingUser });
        }

        // Step 5 - Tenant Created Confirmation
        [Route(SR.CreateTenantConfirmationRoute)]
        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmation([Bind("Email,Password,TenantId,TenantUserName")] OnboardingFlow onboardingFlow)
        {
            return View();
        }
    }
}
