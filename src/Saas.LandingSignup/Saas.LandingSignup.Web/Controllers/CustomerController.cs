using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Saas.LandingSignup.Web.Models;
using Saas.LandingSignup.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Controllers
{
    [Route(SR.ApiRouteTemplate)]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CustomerRepository _customerRepository;
        private readonly TenantRepository _tenantRepository;

        public CustomersController(CustomerRepository customerRepository, TenantRepository tenantRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        public async Task<List<Customer>> GetAll()
        {
            await AddTenantIdToSession();

            return await _customerRepository.GetAllCustomers(HttpContext.Session.GetString(SR.TenantId));
        }

        public async Task AddTenantIdToSession()
        {
            var tenantIdentifier = HttpContext.Session.GetString(SR.TenantId);

            if (string.IsNullOrEmpty(tenantIdentifier))
            {
                var apiKey = HttpContext.Request.Headers[SR.XApiKey].FirstOrDefault();

                if (string.IsNullOrEmpty(apiKey))
                {
                    return;
                }

                Guid apiKeyGuid;

                if (!Guid.TryParse(apiKey, out apiKeyGuid))
                {
                    return;
                }

                var tenantId = await _tenantRepository.GetTenantId(apiKeyGuid); 

                HttpContext.Session.SetString(SR.TenantId, tenantId);
            }
        }
    }
}
