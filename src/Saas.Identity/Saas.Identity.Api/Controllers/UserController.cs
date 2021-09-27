using Microsoft.AspNetCore.Mvc;
using Saas.Identity.Api.Models;

namespace Saas.Identity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ApplicationUser> GetUserById(string userId)
        {
            return null;
        }

        [HttpGet]
        public IEnumerable<ApplicationUser> GetUsersByTenantId(string tenantId)
        {
            return null;
        }

        [HttpPost]
        public string CreateUser(ApplicationUser applicationUser)
        {
            return null;
        }
    }
}
