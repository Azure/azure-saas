using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Models.StateMachine;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Saas.LandingSignup.Web.Controllers
{
    public class OnboardingWorkflowController : Controller
    {
        private readonly ILogger<OnboardingWorkflowController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private OnboardingWorkflowState _workflowState;

        public OnboardingWorkflowController(ILogger<OnboardingWorkflowController> logger, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _workflowState = new OnboardingWorkflowState();
            
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
        public async Task<IActionResult> UsernameAsync(OnboardingWorkflowItem workflowItem)
        {
            ViewBag.EmailAddress = workflowItem.EmailAddress;

            // Do a check to see if username already taken
            var user = new ApplicationUser { UserName = workflowItem.EmailAddress, Email = workflowItem.EmailAddress };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                _workflowState.Transition(OnboardingWorkflowState.Triggers.OnUserNamePosted);

                var onboardingWorkflowItem = new OnboardingWorkflowItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    OnboardingWorkflowName = SR.OnboardingWorkflowName,
                    UserId = user.Id,
                    IsExistingUser = bool.FalseString,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Created = DateTime.Now,
                    WorkflowState = _workflowState
                };

                ViewBag.OnboardingWorkflowItem = onboardingWorkflowItem;

                return RedirectToAction(SR.OrganizationNameAction, SR.OnboardingWorkflowController, new { id = onboardingWorkflowItem.Id, userId = user.Id, isExistingUser = bool.FalseString });
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

        // Step 2 - Submit the organization name
        [HttpGet]
        public IActionResult OrganizationName()//string id, string userId, string isExistingUser, string userNameExists)
        {
            // Populate hidden input fields
            //ViewBag.Id = (string.IsNullOrEmpty(id)) ? Guid.NewGuid().ToString() : id;
            //ViewBag.UserId = userId;
            //ViewBag.IsExistingUser = isExistingUser;
            //ViewBag.userNameExists = userNameExists;

            return View();
        }

        // Step 2 - Submit the organization name
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult OrganizationName(string organizationName)
        {
            _workflowState.Transition(OnboardingWorkflowState.Triggers.OnOrganizationNamePosted);
            ViewBag.OnboardingWorkflowItem.OrganizationName = organizationName;

            return RedirectToAction(SR.OrganizationCategoryAction, SR.OnboardingWorkflowController);
        }

        // Step 2 - Organization Category
        [Route(SR.OnboardingWorkflowOrganizationCategoryRoute)]
        [HttpGet]
        public IActionResult OrganizationCategory(string id, string userId, string isExistingUser, string name)
        {
            // Populate hidden input fields
            ViewBag.Id = id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.Name = name;

            // Populate Categories dropdown list
            var categories = new List<Category>();

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

        // Step 2 Submitted - Organization Category
        [Route(SR.OnboardingWorkflowOrganizationCategoryRoute)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryAsync(string id, string userId, string isExistingUser, string name, int categoryId)
        {
            // Recreate order process id and object
            var item = new OnboardingWorkflowItem()
            {
                Id = id,
                OnboardingWorkflowName = SR.OnboardingWorkflowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            ViewBag.Item = item;

            return RedirectToAction(SR.ServicePlansAction, SR.OnboardingWorkflowController, new { id, userId, isExistingUser, name, categoryId });
        }

        // Step 3 - Service Plan
        [Route(SR.OnboardingWorkflowServicePlansRoute)]
        [HttpGet]
        public IActionResult ServicePlans(string id, string userId, string isExistingUser, string name, int categoryId)
        {
            // Populate hidden input fields
            ViewBag.Id = id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.Name = name;
            ViewBag.CategoryId = categoryId;

            return View();
        }

        // Step 3 Submitted - Service PLan
        [Route(SR.OnboardingWorkflowServicePlansRoute)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ServicePlans(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            // Recreate order process id and object
            var item = new OnboardingWorkflowItem()
            {
                Id = id,
                OnboardingWorkflowName = SR.OnboardingWorkflowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                ProductId = productId,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            ViewBag.Iteam = item;

            return RedirectToAction(SR.DeployTenantAction, SR.CreateTenantController, new { id, userId, isExistingUser, name, categoryId, productId });
        }
    }
}
