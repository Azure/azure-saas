using Microsoft.EntityFrameworkCore;
using Saas.Catalog.Api.Models;
using Saas.Domain.Exceptions;
using Saas.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Saas.Catalog.Api.Services
{
    public class TenantService : ITenantService
    {
        private CatalogDbContext _context;

        public TenantService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task AddItemAsync(Tenant item)
        {
            CatalogTenant catalogTenant = CatalogTenant.FromTenant(item);

            _context.Tenants.Add(catalogTenant);
            await _context.SaveChangesAsync();
            item.Id = catalogTenant.Id;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                throw new TenantNotFoundException();
            }
            else
            {
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tenant> GetItemAsync(Guid id)
        {
            CatalogTenant catalogTenant = await _context.Tenants.FindAsync(id);
            
            if(catalogTenant == null)
            {
                return null;
            }

            return CatalogTenant.ToTenant(catalogTenant);
        }

        public async Task<IEnumerable<Tenant>> GetItemsAsync()
        {
            IEnumerable<CatalogTenant> catalogTenants = await _context.Tenants.ToListAsync();

            IEnumerable<Tenant> tenants = catalogTenants.Select(ct => CatalogTenant.ToTenant(ct));
            return tenants;
        }

        public async Task UpdateItemAsync(Tenant item)
        {

            var catalogTenant = await _context.Tenants.FindAsync(item.Id);
            if (catalogTenant == null)
            {
                throw new TenantNotFoundException();
            }
            catalogTenant.Name = item.Name;
            catalogTenant.UserId = item.UserId;
            catalogTenant.IsActive = item.IsActive;
            catalogTenant.IsCancelled = item.IsCancelled;
            catalogTenant.IsProvisioned = item.IsProvisioned;
            catalogTenant.ApiKey = item.ApiKey;
            catalogTenant.CategoryId = item.CategoryId;

            _context.Tenants.Update(catalogTenant);

            await _context.SaveChangesAsync();
        }

    }
}

