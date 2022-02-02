using Microsoft.AspNetCore.Mvc;
using Saas.Admin.Web.Models;
using Saas.Admin.Web.Services;
using System;
using System.Threading.Tasks;

namespace Saas.Admin.Web.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        // GET: Tenants
        public async Task<IActionResult> Index()
        {
            return View(await _tenantService.GetItemsAsync());
        }

        // GET: Tenants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _tenantService.GetItemAsync(id.Value);
            if (tenant == null)
            {
                return NotFound();
            }

            return View(tenant);
        }

        // GET: Tenants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tenants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsActive,IsCancelled,IsProvisioned,ApiKey,CategoryId,ProductId,UserId,Created")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                await _tenantService.AddItemAsync(tenant);
                return RedirectToAction(nameof(Index));
            }
            return View(tenant);
        }

        // GET: Tenants/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _tenantService.GetItemAsync(id.Value);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,IsActive,IsCancelled,IsProvisioned,ApiKey,CategoryId,ProductId,UserId,Created")] Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var lookupTenant = await _tenantService.GetItemAsync(id);
                if (lookupTenant == null)
                {
                    return NotFound();
                }
                await _tenantService.UpdateItemAsync(tenant);
                return RedirectToAction(nameof(Index));
            }
            return View(tenant);
        }

        // GET: Tenants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _tenantService.GetItemAsync(id.Value);
            if (tenant == null)
            {
                return NotFound();
            }
            return View(tenant);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tenant = await _tenantService.GetItemAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            await _tenantService.DeleteItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
