using Microsoft.EntityFrameworkCore;
using Saas.Admin.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saas.Admin.Web.Services


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
            _context.Tenants.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid Id)
        {
            var tenant = await GetItemAsync(Id);
            if (tenant == null)
            {
                // TODO: throw not found exception
            }
            else
            {
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tenant> GetItemAsync(Guid id)
        {
            return await _context.Tenants.FindAsync(id);
        }

        public async Task<IEnumerable<Tenant>> GetItemsAsync()
        {
            return await _context.Tenants.ToListAsync();
        }

        public async Task UpdateItemAsync(Tenant item)
        {
            if (!await _context.Tenants.AnyAsync(t => t.Id == item.Id))
            {
                // TODO: throw not found exception
                return;
            }
            _context.Tenants.Update(item);
            await _context.SaveChangesAsync();
        }

    }
}

