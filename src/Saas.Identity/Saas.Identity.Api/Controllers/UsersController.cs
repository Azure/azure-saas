using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas.Identity.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saas.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        // GET api/Users/5
        [HttpGet("{id}")]
        public async Task<IdentityUser> GetUserById(string id)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);

            return applicationUser;
        }

        // Post: api/Users/
        [HttpPost]
        public async Task PostAsync([FromBody] ApplicationUser applicationUser)
        {
            var result = await _userManager.CreateAsync(applicationUser);

            if (result.Succeeded)
            {
            }
        }
    }
}
