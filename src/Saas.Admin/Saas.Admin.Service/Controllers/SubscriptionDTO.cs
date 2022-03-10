namespace Saas.Admin.Service.Controllers;

public class TenantDTO
{
    /// <summary>
    /// For serialization don't use this
    /// </summary>
    public TenantDTO()
    {
        Name = String.Empty;
        CreatedBy = String.Empty;
        Version = String.Empty;
    }

    public TenantDTO(Tenant tenant)
    {
        Id = tenant.Id;
        IsActive = tenant.IsActive;
        IsCancelled = tenant.IsCancelled;
        IsProvisioned = tenant.IsProvisioned;
        ApiKey = tenant.ApiKey;
        CategoryId = tenant.CategoryId;
        ProductId = tenant.ProductId;
        CreatedTime = tenant.CreatedTime;

        Name = Guard.Argument(tenant.Name, nameof(tenant.Name)).NotEmpty();
        CreatedBy = Guard.Argument(tenant.CreatedBy, nameof(tenant.CreatedBy)).NotEmpty();
        Version = Convert.ToBase64String(Guard.Argument(tenant.ConcurrencyToken, nameof(tenant.ConcurrencyToken)).NotEmpty());
    }

    public Tenant ToTenant()
    {

        byte[] concurrentcyToken = Convert.FromBase64String(this.Version);
        Tenant tenant = new Tenant(this.Name, this.CreatedBy, concurrentcyToken)
        {
            Id = this.Id,
            IsActive = this.IsActive,
            IsCancelled = this.IsCancelled,
            IsProvisioned = this.IsProvisioned,
            ApiKey = this.ApiKey,
            CategoryId = this.CategoryId,
            ProductId = this.ProductId,
            CreatedTime = this.CreatedTime,
        };
        return tenant;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsProvisioned { get; set; }
    public Guid ApiKey { get; set; }
    public int CategoryId { get; set; }
    public int ProductId { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Version { get; set; }
}

public class TenantDTOPage
{
    public TenantDTOPage(IEnumerable<TenantDTO> tenants, int totalCount, int startIndex)
    {
        Tenants = tenants;
        TotalCount = totalCount;
        StartIndex = startIndex;
    }

    public IEnumerable<TenantDTO> Tenants { get; }
    public int TotalCount { get; }
    public int StartIndex { get; }
}
