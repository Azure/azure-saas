using Dawn;

using Saas.Domain.Models;

using System;
using System.ComponentModel.DataAnnotations;

namespace Saas.Catalog.Api.Models
{
    public class CatalogTenant
    {
        public CatalogTenant(string name, string userId)
        {
            this.Name = Guard.Argument(name, nameof(name)).NotEmpty();
            this.UserId = Guard.Argument(userId, nameof(userId)).NotEmpty();

        }
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsProvisioned { get; set; }
        public Guid ApiKey { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
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
