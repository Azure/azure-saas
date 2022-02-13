using Microsoft.EntityFrameworkCore;
using Saas.Catalog.Api.Models;
using Saas.Domain.Exceptions;
using Saas.Domain.Models;
using System;
using System.Collections.Generic;
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
            //_context.Tenants.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid Id)
        {
            var tenant = await GetItemAsync(Id);
            if (tenant == null)
            {
                throw new TenantNotFoundException();
            }
            else
            {
                //_context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tenant> GetItemAsync(Guid id)
        {
            //return await _context.Tenants.FindAsync(id);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tenant>> GetItemsAsync()
        {
            //return await _context.Tenants.ToListAsync();
            throw new NotImplementedException();
        }

        public async Task UpdateItemAsync(Tenant item)
        {
            throw new NotImplementedException();

            //var tenant = await GetItemAsync(item.Id);
            //if (tenant == null)
            //{
            //    throw new TenantNotFoundException();
            //}
            //tenant.Name = item.Name;
            //tenant.UserId = item.UserId;
            //tenant.IsActive = item.IsActive;
            //tenant.IsCancelled = item.IsCancelled;  
            //tenant.IsProvisioned = item.IsProvisioned;  
            //tenant.ApiKey = item.ApiKey;
            //tenant.CategoryId = item.CategoryId;
            //_context.Tenants.Update(tenant);
            //await _context.SaveChangesAsync();
        }

    }
}

