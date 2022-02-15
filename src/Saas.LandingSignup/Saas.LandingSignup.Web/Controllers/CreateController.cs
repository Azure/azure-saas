﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Models.CosmosDb;
using Saas.LandingSignup.Web.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Controllers
{
    public class CreateController : Controller
    {
        private const string AuthorityFormat = "https://login.microsoftonline.com/{0}/v2.0";

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

        [Route("/create")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/create/name")]
        public IActionResult Name(string id, string userId, string isExistingUser, string userNameExists)
        {
            if (string.IsNullOrEmpty(id))
            {
                string processId = Guid.NewGuid().ToString();
                ViewBag.Id = processId;
            }
            else
            {
                ViewBag.Id = id;
            }

            // Populate hidden input fields
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.userNameExists = userNameExists;

            return View();
        }

        [Route("/create/name")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NameAsync(string id, string userId, string isExistingUser, string name)
        {
            // Create order process id and object
            Item item = new Item()
            {
                Id = id,
                Name = "Onboarding Flow",
                TenantName = name,
                UserId = userId,
                IsExistingUser = isExistingUser,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now
            };

            // Commit to CosmosDB
            try
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return RedirectToAction("category", "create", new { item.Id, userId, isExistingUser, name });
        }

        [Route("/create/category")]
        public IActionResult Category(string id, string userId, string isExistingUser, string name)
        {
            // Populate hidden input fields
            ViewBag.Id = id;
            ViewBag.UserId = userId;
            ViewBag.IsExistingUser = isExistingUser;
            ViewBag.Name = name;

            // Populate Categories dropdown list
            List<Category> categories = new List<Category>();
            categories.Add(new Category { Id = 1, Name = "Automotive, Mobility & Transportation" });
            categories.Add(new Category { Id = 2, Name = "Energy & Sustainability" });
            categories.Add(new Category { Id = 3, Name = "Financial Services" });
            categories.Add(new Category { Id = 4, Name = "Healthcare & Life Sciences" });
            categories.Add(new Category { Id = 5, Name = "Manufacturing & Supply Chain" });
            categories.Add(new Category { Id = 6, Name = "Media & Communications" });
            categories.Add(new Category { Id = 7, Name = "Public Sector" });
            categories.Add(new Category { Id = 8, Name = "Retail & Consumer Goods" });
            categories.Add(new Category { Id = 9, Name = "Software" });

            return View(categories);
        }

        [Route("/create/category")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryAsync(string id, string userId, string isExistingUser, string name, int categoryId)
        {
            // Recreate order process id and object
            Item item = new Item()
            {
                Id = id,
                Name = "Onboarding Flow",
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

            return RedirectToAction("plans", "create", new { id, userId, isExistingUser, name, categoryId });
        }

        [Route("/create/plans")]
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

        [Route("/create/plans")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlansAsync(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            // Recreate order process id and object
            Item item = new Item()
            {
                Id = id,
                Name = "Onboarding Flow",
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
            }
        }

        [Route("/create/merchant")]
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

        [Route("/create/deploy")]
        public async Task<IActionResult> DeployAsync(string id, string userId, string isExistingUser, string name, int categoryId, int productId)
        {
            HttpClient httpClient = new HttpClient();
            OnboardingClient onboardingClient = new OnboardingClient(_appSettings.OnboardingApiBaseUrl, httpClient);

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
            Item item = new Item()
            {
                Id = id,
                Name = "Onboarding Flow",
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

            return RedirectToAction("confirmation", "create", new { isExistingUser });
        }

        [Route("/create/confirmation")]
        public IActionResult Confirmation(string isExistingUser)
        {
            return View();
        }

        [Route("/create/confirmation")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmation([Bind("Email,Password,TenantId,TenantUserName")] OnboardingFlow onboardingFlow)
        {
            return View();
        }

        protected int CreateCustomer(string email)
        {
            // Create customer 
            using (SqlConnection connection = new SqlConnection("Data Source=tcp:ma.database.windows.net;Initial Catalog=ma-provider-sql;User Id=modernappz;Password=N0sK3Tamanda;"))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("usp_CreateCustomer", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@email", email);
                    //cmd.Parameters.AddWithValue("@userId", User.Identity.GetUserId());

                    SqlParameter returnValue = new SqlParameter();
                    returnValue.Direction = System.Data.ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();

                    int customerId = (int)returnValue.Value;

                    return customerId;
                }
            }
        }

        protected int CreateOrder(string userId, int productId)
        {
            // Create order
            using (SqlConnection connection = new SqlConnection("Data Source=tcp:ma.database.windows.net;Initial Catalog=ma-provider-sql;User Id=modernappz;Password=N0sK3Tamanda;"))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("usp_CreateOrder", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlParameter returnValue = new SqlParameter();
                    returnValue.Direction = System.Data.ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();

                    int orderId = (int)returnValue.Value;

                    return orderId;
                }
            }
        }

        private string Clean(string s)
        {
            return new StringBuilder(s)
                  .Replace("&", "and")
                  .Replace(",", "")
                  .Replace("  ", "")
                  .Replace(" ", "")
                  .Replace("'", "")
                  .Replace(".", "")
                  .ToString()
                  .ToLower();
        }
    }
}