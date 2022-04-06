using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas.Identity.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetRolesController : ControllerBase
    {
        private readonly IdentityDbContext _context;

        public AspNetRolesController(IdentityDbContext context)
        {
            _context = context;
        }

        // GET: api/AspNetRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetRole>>> GetAspNetRoles()
        {
            return await _context.AspNetRoles.ToListAsync();
        }

        // GET: api/AspNetRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetRole>> GetAspNetRole(string id)
        {
            var aspNetRole = await _context.AspNetRoles.FindAsync(id);

            if (aspNetRole == null)
            {
                return NotFound();
            }

            return aspNetRole;
        }

        // PUT: api/AspNetRoles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetRole(string id, AspNetRole aspNetRole)
        {
            if (id != aspNetRole.Id)
            {
                return BadRequest();
            }

            _context.Entry(aspNetRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRoleExists(id))
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

        // POST: api/AspNetRoles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AspNetRole>> PostAspNetRole(AspNetRole aspNetRole)
        {
            _context.AspNetRoles.Add(aspNetRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetRoleExists(aspNetRole.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAspNetRole", new { id = aspNetRole.Id }, aspNetRole);
        }

        // DELETE: api/AspNetRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetRole(string id)
        {
            var aspNetRole = await _context.AspNetRoles.FindAsync(id);
            if (aspNetRole == null)
            {
                return NotFound();
            }

            _context.AspNetRoles.Remove(aspNetRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AspNetRoleExists(string id)
        {
            return _context.AspNetRoles.Any(e => e.Id == id);
        }
    }
}
