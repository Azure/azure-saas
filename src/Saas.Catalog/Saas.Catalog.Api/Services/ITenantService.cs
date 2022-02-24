using Saas.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saas.Catalog.Api.Services
{
    /// <summary>
    /// Data access interface for tenant related CRUD
    /// </summary>
    public interface ITenantService 
    {
        /// <summary>
        /// Get a list of all tenants
        /// </summary>
        /// <returns>Enumerable list of tenants (can be empty)</returns>
        Task<IEnumerable<Tenant>> GetItemsAsync();
        /// <summary>
        /// Get a specific tenant by id
        /// </summary>
        /// <param name="id">Id of tenant</param>
        /// <returns>Tenant if found or NULL</returns>
        Task<Tenant?> GetItemAsync(Guid id);
        /// <summary>
        /// Add a new tenant to the database or throw an exception
        /// </summary>
        /// <param name="item">New tenant information</param>
        /// <returns>Updated tenant</returns>
        /// <remarks>Tenant ID must be unique or will throw exception</remarks>
        Task<Tenant> AddItemAsync(Tenant item);
        /// <summary>
        /// Update values in a tenant
        /// </summary>
        /// <param name="item">Complete instance of the tenant</param>
        /// <returns></returns>
        /// <remarks>Tenant ID must exist or will throw exception</remarks>
        Task UpdateItemAsync(Tenant item);
        /// <summary>
        /// Delete a tenant
        /// </summary>
        /// <param name="id">Tenant ID</param>
        /// <returns></returns>
        /// <remarks>Tenant ID must exists or will throw exception</remarks>
        Task DeleteItemAsync(Guid id);
    }
}
