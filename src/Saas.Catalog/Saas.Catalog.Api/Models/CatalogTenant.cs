using Dawn;

using Saas.Domain.Models;

using System;
using System.ComponentModel.DataAnnotations;

namespace Saas.Catalog.Api.Models
{
    /// <summary>
    /// Entity definition for a tenant in the catalog
    /// </summary>
    public class CatalogTenant
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the tenant</param>
        /// <param name="userId">ID of the user who created it</param>
        public CatalogTenant(string name, string userId)
        {
            this.Name = Guard.Argument(name, nameof(name)).NotEmpty();
            this.UserId = Guard.Argument(userId, nameof(userId)).NotEmpty();
        }
        /// <summary>
        /// Id of the user who created this tenant
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Tenant display name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Wether the tenant is active or not
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Wether this subscription has been canceled or not
        /// </summary>
        public bool IsCancelled { get; set; }
        /// <summary>
        /// Provisioning status
        /// </summary>
        public bool IsProvisioned { get; set; }
        /// <summary>
        /// Tenant API Key
        /// </summary>
        public Guid ApiKey { get; set; }
        /// <summary>
        /// Tenant Category
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Product selected
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// User who created the tenant
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Time this tenant was created
        /// </summary>
        public DateTime Created { get; set; }

        internal static CatalogTenant FromTenant(Tenant tenant)
        {
            CatalogTenant catalogTenant = new CatalogTenant(tenant.Name, tenant.UserId);
            catalogTenant.Id = tenant.Id;
            catalogTenant.Name = tenant.Name;
            catalogTenant.IsActive = tenant.IsActive;   
            catalogTenant.IsCancelled = tenant.IsCancelled;
            catalogTenant.IsProvisioned = tenant.IsProvisioned;
            catalogTenant.ApiKey = tenant.ApiKey;
            catalogTenant.CategoryId = tenant.CategoryId;
            catalogTenant.ProductId = tenant.ProductId;
            catalogTenant.UserId = tenant.UserId;
            catalogTenant.Created = tenant.Created;

            return catalogTenant;
        }

        internal static Tenant ToTenant(CatalogTenant catalogTenant)
        {
            Tenant tenant = new Tenant(catalogTenant.Name, catalogTenant.UserId);
            tenant.Id = catalogTenant.Id;
            tenant.Name = catalogTenant.Name;
            tenant.IsActive = catalogTenant.IsActive;
            tenant.IsCancelled = catalogTenant.IsCancelled;
            tenant.IsProvisioned = catalogTenant.IsProvisioned;
            tenant.ApiKey = catalogTenant.ApiKey;
            tenant.CategoryId = catalogTenant.CategoryId;
            tenant.ProductId = catalogTenant.ProductId;
            tenant.UserId = catalogTenant.UserId;
            tenant.Created = catalogTenant.Created;

            return tenant;
        }

    }
}
