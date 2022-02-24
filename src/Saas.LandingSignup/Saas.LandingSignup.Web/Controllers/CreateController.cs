using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Models.CosmosDb;
using Saas.LandingSignup.Web.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Controllers
{
    public class CreateController : Controller
    {
        private readonly ILogger<CreateController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICosmosDbService _cosmosDbService;

        public CreateController(ILogger<CreateController> logger, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _cosmosDbService = cosmosDbService;
        }

        [Route("/" + SR.CreateController)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Step 1
        [Route("/" + SR.CreateController + "/" + SR.NameAction)]
        [HttpGet]
        public IActionResult Name(string id, string userId, string isExistingUser, string userNameExists)
        {
            // Populate hidden input fields
            ViewBag.Id = (string.IsNullOrEmpty(id)) ? Guid.NewGuid().ToString() : id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.userNameExists = userNameExists;

            return View();
        }

        // Step 1
        [Route("/" + SR.CreateController + "/" + SR.NameAction)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NameAsync(string id, string userId, string isExistingUser, string name)
        {
            // Create order process id and object
            var item = new Item()
            {
                Id = id,
                Name = SR.OnboardingFlowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            // Commmit to CosmosDB
            try
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return RedirectToAction(SR.CategoryAction,SR.CreateController, new { item.Id, userId, isExistingUser, name });
        }

        [Route("/" + SR.CreateController+ "/"+ SR.CategoryAction)]
        [HttpGet]
        public IActionResult Category(string id, string userId, string isExistingUser, string name)
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

        [Route("/" + SR.CreateController + "/" + SR.CategoryAction)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryAsync(string id, string userId, string isExistingUser, string name, int categoryId)
        {
            // Recreate order process id and object
            var item = new Item()
            {
                Id = id,
                Name = SR.OnboardingFlowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            // Update order process in CosmosDB
            try
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return RedirectToAction(SR.PlansAction, SR.CreateController, new { id, userId, isExistingUser, name, categoryId });
        }

        [Route("/" + SR.CreateController+"/"+SR.PlansAction)]
        [HttpGet]
        public IActionResult Plans(string id, string userId, string isExistingUser, string name, int categoryId)
        {
            // Populate hidden input fields
            ViewBag.Id = id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.Name = name;
            ViewBag.CategoryId = categoryId;

            return View();
        }

        [Route("/" + SR.CreateController + "/" + SR.PlansAction)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlansAsync(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            // Recreate order process id and object
            var item = new Item()
            {
                Id = id,
                Name = SR.OnboardingFlowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                ProductId = productId,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            // Update order process in CosmosDB
            try
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            /* Converting this flow to use Deploy route only. This will bypass the Stripe could and prevent the calls. 
             * The result will be always going to the confirmation page regardless of tier chosen
            if (productId == 5)
            {
                return RedirectToAction("deploy", "create", new { id, userId, isExistingUser, name, categoryId, productId });
            }
            else if (productId == 6)
            {
                return RedirectToAction("merchant", "create", new { id, userId, isExistingUser, name, categoryId, productId });
            }
            else
            {
                return RedirectToAction("merchant", "create", new { id, userId, isExistingUser, name, categoryId, productId });
            }*/

            return RedirectToAction(SR.DeployAction, SR.CreateController, new { id, userId, isExistingUser, name, categoryId, productId });
        }

        [Route("/" + SR.CreateController + "/" + SR.MerchantAction)]
        [HttpGet]
        public IActionResult Merchant(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            // Populate hidden input fields
            ViewBag.UserId = id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.Name = name;
            ViewBag.CategoryId = categoryId;
            ViewBag.ProductId = productId;
            ViewBag.StripeProductPlanSubscriberBasic = _appSettings.StripeProductPlanSubscriberBasic;
            ViewBag.StripeProductPlanSubscriberStandard = _appSettings.StripeProductPlanSubscriberStandard;
            ViewBag.StripePublishableKey = _appSettings.StripePublishableKey;

            return View();
        }

        [Route("/" + SR.CreateController + "/" + SR.DeployAction)]
        [HttpGet]
        public async Task<IActionResult> DeployAsync(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
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
            var item = new Item()
            {
                Id = id,
                Name = SR.OnboardingFlowName,
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                CategoryId = categoryId,
                ProductId = productId,
                IsComplete = true,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            // Update order process in CosmosDB
            try
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return RedirectToAction(SR.ConfirmationAction, SR.CreateController, new { isExistingUser });
        }

        [Route("/" + SR.CreateController + "/" + SR.ConfirmationAction)]
        [HttpGet]
        public IActionResult Confirmation(string isExistingUser)
        {
            return View();
        }

        [Route("/" + SR.CreateController + "/" + SR.ConfirmationAction)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmation([Bind("Email,Password,TenantId,TenantUserName")] OnboardingFlow onboardingFlow)
        {
            return View();
        }
    }
}
