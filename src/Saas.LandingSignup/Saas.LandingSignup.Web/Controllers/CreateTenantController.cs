using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Saas.LandingSignup.Web.Models.StateMachine;

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

        // Step 5 - Submitted Tenant Creation
        [HttpGet]
        public async Task<IActionResult> DeployTenantAsync()
        {
            var httpClient = new HttpClient();
            var onboardingClient = new OnboardingClient(_appSettings.OnboardingApiBaseUrl, httpClient);

            var workflowItem = HttpContext.Session.GetObjectFromJson<OnboardingWorkflowItem>(SR.OnboardingWorkflowItemKey);

            Services.Tenant tenant = new Services.Tenant()
            {
                Id = Guid.NewGuid(),
                Name = workflowItem.Id,
                IsActive = true,
                IsCancelled = false,
                IsProvisioned = true,
                ApiKey = Guid.NewGuid(),
                CategoryId = workflowItem.CategoryId,
                ProductId = workflowItem.ProductId,
                UserId = workflowItem.UserId
            };

            await onboardingClient.TenantsPOSTAsync(tenant);

            workflowItem.IsComplete = true;
            workflowItem.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            workflowItem.Created = DateTime.Now;
            workflowItem.CurrentWorkflowState = OnboardingWorkflowState.States.TenantDeploymentConfirmation;

            HttpContext.Session.SetObjectAsJson(SR.OnboardingWorkflowItemKey, workflowItem);

            return RedirectToAction(SR.ConfirmationAction, SR.CreateTenantController);
        }

        // Step 6 - Tenant Created Confirmation
        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
