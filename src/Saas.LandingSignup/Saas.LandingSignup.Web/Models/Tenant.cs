using System;

namespace Saas.Provider.Web.Models
{
    public class Tenant
    {
        public string Id { get; set; }
        public Guid ApiIKey { get; set; }
        public string TenantName { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseServer { get; set; }
        public string WebAppName { get; set; }
        public string StorageContainerName { get; set; }
        public string Url { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsProvisioned { get; set; }
        public bool IsCancelled { get; set; }
        public int ProductId { get; set; }
        public string ProvisioningStatus { get; set; }
        public string ProductName { get; set; }
        public string ProductTier { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; }
        public string Region { get; set; }
        public string IpAddress { get; set; }
        public int OrderId { get; set; }
        public Owner Owner { get; set; }
        public string UserName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
