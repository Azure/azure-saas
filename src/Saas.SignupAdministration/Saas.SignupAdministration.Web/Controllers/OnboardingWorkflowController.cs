using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saas.SignupAdministration.Web.Models;
using Saas.SignupAdministration.Web.Models.StateMachine;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Saas.SignupAdministration.Web.Services;

namespace Saas.SignupAdministration.Web.Controllers
{
    public class OnboardingWorkflowController : Controller
    {
        private readonly ILogger<OnboardingWorkflowController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OnboardingWorkflowState _workflowState;
        private readonly OnboardingWorkflowItem _workflowItem;

        public OnboardingWorkflowController(ILogger<OnboardingWorkflowController> logger, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _workflowState = new OnboardingWorkflowState();
            _workflowItem = new OnboardingWorkflowItem();

            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Step 1 - Submit the email and determine if it is in use
        [HttpGet]
        public IActionResult Username()
        {
            return View();
        }

        // Step 1 - Submit the email and determine if it is in use
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UsernameAsync(string emailAddress)
        {
            // Do a check to see if username already taken
            ApplicationUser user = new ApplicationUser { UserName = emailAddress, Email = emailAddress };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                _workflowItem.Id = Guid.NewGuid().ToString();
                _workflowItem.OnboardingWorkflowName = SR.OnboardingWorkflowName;
                _workflowItem.UserId = user.Id;
                _workflowItem.EmailAddress = emailAddress;
                _workflowItem.IsExistingUser = bool.FalseString;
                _workflowItem.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                _workflowItem.Created = DateTime.Now;
                UpdateSessionAndTranstionState(_workflowItem, OnboardingWorkflowState.Triggers.OnUserNamePosted);

                return RedirectToAction(SR.OrganizationNameAction, SR.OnboardingWorkflowController);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.ToLower() == SR.DuplicateUserNameErrorCode.ToLower())
                    {
                        ViewBag.ErrorMessage = error.Description;
                    }

                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }
        }

        // Step 2 - Submit the organization name
        [HttpGet]
        public IActionResult OrganizationName()
        {
            return View();
        }

        // Step 2 - Submit the organization name
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult OrganizationName(string organizationName)
        {
            var workflowItem = GetOnboardingWorkflowItemFromSession();

            workflowItem.OrganizationName = organizationName;
            UpdateSessionAndTranstionState(workflowItem, OnboardingWorkflowState.Triggers.OnOrganizationNamePosted);

            return RedirectToAction(SR.OrganizationCategoryAction, SR.OnboardingWorkflowController);
        }

        // Step 3 - Organization Category
        [Route(SR.OnboardingWorkflowOrganizationCategoryRoute)]
        [HttpGet]
        public IActionResult OrganizationCategory()
        {
            // Populate Categories dropdown list
            List<Category> categories = new List<Category>();

            categories.Add(new Category { Id = 1, Name = SR.AutomotiveMobilityAndTransportationPrompt });
            categories.Add(new Category { Id = 2, Name = SR.EnergyAndSustainabilityPrompt });
            categories.Add(new Category { Id = 3, Name = SR.FinancialServicesPrompt });
            categories.Add(new Category { Id = 4, Name = SR.HealthcareAndLifeSciencesPrompt });
            categories.Add(new Category { Id = 5, Name = SR.ManufacturingAndSupplyChainPrompt });
            categories.Add(new Category { Id = 6, Name = SR.MediaAndCommunicationsPrompt });
            categories.Add(new Category { Id = 7, Name = SR.PublicSectorPrompt });
            categories.Add(new Category { Id = 8, Name = SR.RetailAndConsumerGoodsPrompt });
            categories.Add(new Category { Id = 9, Name = SR.SoftwarePrompt });

            return View(categories);
        }

        // Step 3 Submitted - Organization Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationCategoryAsync(int categoryId)
        {
            var workflowItem = GetOnboardingWorkflowItemFromSession();

            workflowItem.CategoryId = categoryId;
            UpdateSessionAndTranstionState(workflowItem, OnboardingWorkflowState.Triggers.OnOrganizationCategoryPosted);

            return RedirectToAction(SR.TenantRouteNameAction, SR.OnboardingWorkflowController);
        }

        // Step 4 - Tenant Route Name
        [HttpGet]
        public IActionResult TenantRouteName()
        {
            return View();
        }

        // Step 4 Submitted - Tenant Route Name
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TenantRouteName(string tenantRouteName)
        {
            // TODO:Need to check whether the route name exists

            var workflowItem = GetOnboardingWorkflowItemFromSession();

            workflowItem.TenantRouteName = tenantRouteName;
            UpdateSessionAndTranstionState(workflowItem, OnboardingWorkflowState.Triggers.OnTenantRouteNamePosted);

            return RedirectToAction(SR.ServicePlansAction, SR.OnboardingWorkflowController);
        }

        // Step 5 - Service Plan
        [HttpGet]
        public IActionResult ServicePlans()
        {
            return View();
        }

        // Step 5 Submitted - Service Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ServicePlans(int productId)
        {
            var workflowItem = GetOnboardingWorkflowItemFromSession();

            workflowItem.ProductId = productId;
            UpdateSessionAndTranstionState(workflowItem, OnboardingWorkflowState.Triggers.OnServicePlanPosted);

            return RedirectToAction(SR.ConfirmationAction, SR.OnboardingWorkflowController);
        }

        // Step 6 - Tenant Created Confirmation
        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            // Deploy the Tenant
            await DeployTenantAsync();

            return View();
        }


        private async Task DeployTenantAsync()
        {
            HttpClient httpClient = new HttpClient();
            OnboardingClient onboardingClient = new OnboardingClient(_appSettings.OnboardingApiBaseUrl, httpClient);

            var workflowItem = GetOnboardingWorkflowItemFromSession();

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

            UpdateSessionAndTranstionState(workflowItem, OnboardingWorkflowState.Triggers.OnTenantDeploymentSuccessful);
        }

        private OnboardingWorkflowItem GetOnboardingWorkflowItemFromSession()
        {
            return HttpContext.Session.GetObjectFromJson<OnboardingWorkflowItem>(SR.OnboardingWorkflowItemKey);
        }

        private void UpdateSessionAndTranstionState(OnboardingWorkflowItem workflowItem, OnboardingWorkflowState.Triggers trigger)
        {
            _workflowState.CurrentState = workflowItem.CurrentWorkflowState;
            workflowItem.CurrentWorkflowState = _workflowState.Transition(trigger);
            HttpContext.Session.SetObjectAsJson(SR.OnboardingWorkflowItemKey, workflowItem);
        }
    }
}
