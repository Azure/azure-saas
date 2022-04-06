using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saas.Identity.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUsersController : ControllerBase
    {
        private readonly ILogger<AspNetUsersController> _logger;
        private readonly IdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetUsersController(ILogger<AspNetUsersController> logger, IdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: api/AspNetUsers
        [HttpGet]
        public async Task<IEnumerable<AspNetUser>> GetAspNetUsersAsync()
        {
            return await _context.AspNetUsers.ToListAsync();
        }

        // GET: api/AspNetUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetAspNetUser(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        // GET: api/TenantAspNetUsers/75eafad2-9dfc-4289-84c4-de25a1a1e19d
        [Route("/api/TenantAspNetUsers/{tenantId}")]
        [HttpGet]
        public async Task<IEnumerable<AspNetUser>> GetAspNetUsersByTenantId(string tenantId)
        {
            var aspNetusers = await _context.AspNetUsers
                        .Where(a => a.TenantId == tenantId)
                        .ToListAsync();

            return aspNetusers;
        }

        // PUT: api/AspNetUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetUser(string id, AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(aspNetUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AspNetUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AspNetUser>> PostAspNetUser(AspNetUser aspNetUser)
        {
            _context.AspNetUsers.Add(aspNetUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserExists(aspNetUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAspNetUser", new { id = aspNetUser.Id }, aspNetUser);
        }

        // DELETE: api/AspNetUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetUser(string id)
        {
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            _context.AspNetUsers.Remove(aspNetUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AspNetUserExists(string id)
        {
            return _context.AspNetUsers.Any(e => e.Id == id);
        }
    }
}
