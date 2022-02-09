using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saas.Catalog.Api.Services;
using Saas.Domain.Exceptions;
using Saas.Domain.Models;

namespace Saas.Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        // GET: api/Tenants
        [HttpGet]
        public async Task<IEnumerable<Tenant>> GetTenants()
        {
            return await _tenantService.GetItemsAsync();
        }

        // GET: api/Tenants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetTenant(Guid id)
        {
            var tenant = await _tenantService.GetItemAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        // PUT: api/Tenants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTenant(Guid id, Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return BadRequest();
            }

            try
            {
                await _tenantService.UpdateItemAsync(tenant);
            }
            catch (TenantNotFoundException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST: api/Tenants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tenant>> PostTenant(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                // TODO use exceptions or tweak checking method
                var dbtenant = await _tenantService.GetItemAsync(tenant.Id);
                if (dbtenant != null)
                {
                    return Conflict();
                }
                else
                {
                    await _tenantService.AddItemAsync(tenant);
                }

            }

            return CreatedAtAction("GetTenant", new { id = tenant.Id }, tenant);
        }

        // DELETE: api/Tenants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            try
            {
                await _tenantService.DeleteItemAsync(id);
            }
            catch (TenantNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
