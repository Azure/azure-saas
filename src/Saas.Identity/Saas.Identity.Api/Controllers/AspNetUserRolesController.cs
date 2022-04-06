using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas.Identity.Api.Models;

namespace Saas.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUserRolesController : ControllerBase
    {
        private readonly IdentityDbContext _context;
        private RoleManager<AspNetRole> _roleManager;

        public AspNetUserRolesController(IdentityDbContext context, RoleManager<AspNetRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GET: api/AspNetUserRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetUserRole>>> GetAspNetUserRole()
        {
            return await _context.AspNetUserRoles.ToListAsync();
        }

        // GET: api/AspNetUserRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetUserRole>> GetAspNetUserRole(string id)
        {
            var aspNetUserRoles = await _context.AspNetUserRoles.FindAsync(id);

            if (aspNetUserRoles == null)
            {
                return NotFound();
            }

            return aspNetUserRoles;
        }

        // PUT: api/AspNetUserRoles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetUserRole(string id, AspNetUserRole aspNetUserRole)
        {
            if (id != aspNetUserRole.UserId)
            {
                return BadRequest();
            }

            _context.Entry(aspNetUserRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserRoleExists(id))
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

        // POST: api/AspNetUserRoles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AspNetUserRole>> PostAspNetUserRole(AspNetUserRole aspNetUserRole)
        {
            _context.AspNetUserRole.Add(aspNetUserRole);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserRoleExists(aspNetUserRole.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAspNetUserRole", new { id = aspNetUserRole.UserId }, aspNetUserRole);
        }

        // DELETE: api/AspNetUserRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetUserRole(string id)
        {
            var aspNetUserRole = await _context.AspNetUserRole.FindAsync(id);
            if (aspNetUserRole == null)
            {
                return NotFound();
            }

            _context.AspNetUserRole.Remove(aspNetUserRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AspNetUserRoleExists(string id)
        {
            return _context.AspNetUserRole.Any(e => e.UserId == id);
        }
    }
}
