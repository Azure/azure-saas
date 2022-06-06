namespace Saas.SignupAdministration.Web.Models
{
    public class Tenant
    {
        public string Id { get; set; } = string.Empty;
        public Guid ApiIKey { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseServer { get; set; } = string.Empty;
        public string WebAppName { get; set; } = string.Empty;
        public string StorageContainerName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public bool IsProvisioned { get; set; }
        public bool IsCancelled { get; set; }
        public int ProductId { get; set; }
        public string ProvisioningStatus { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductTier { get; set; } = string.Empty;
        public int Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public Owner? Owner { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
    }
}
