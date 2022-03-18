namespace Saas.Admin.Service.Controllers;

public class TenantDTO
{
    /// <summary>
    /// For serialization don't use this
    /// </summary>
    public TenantDTO()
    {
        Name = String.Empty;
        RoutePrefix = string.Empty;
        Version = String.Empty;
    }

    public TenantDTO(Tenant tenant)
    {
        Id = tenant.Id;
        IsCancelled = tenant.IsCancelled;
        IsProvisioned = tenant.IsProvisioned;

        CreatedTime = Guard.Argument(tenant.CreatedTime, nameof(tenant.CreatedTime)).NotNull();

        Name = Guard.Argument(tenant.Name, nameof(tenant.Name)).NotEmpty();
        RoutePrefix = Guard.Argument(tenant.RoutePrefix, nameof(tenant.RoutePrefix)).NotEmpty();

        Version = Convert.ToBase64String(Guard.Argument(tenant.ConcurrencyToken, nameof(tenant.ConcurrencyToken)).NotEmpty());
    }

    public Tenant ToTenant()
    {
        Tenant tenant = new Tenant(this.Name, this.RoutePrefix)
        {
            Id = this.Id,
            IsCancelled = this.IsCancelled,
            IsProvisioned = this.IsProvisioned,
            ConcurrencyToken = this.Version != null ? Convert.FromBase64String(this.Version) : null,
            CreatedTime = null,
        };
        return tenant;
    }


    public void CopyTo(Tenant target)
    {
        target.Name = this.Name;
        target.IsCancelled = this.IsCancelled;
        target.IsProvisioned = this.IsProvisioned;
        target.RoutePrefix = this.RoutePrefix;
        target.CreatedTime = null;
        target.ConcurrencyToken = this.Version != null  ? Convert.FromBase64String(this.Version) : null;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsProvisioned { get; set; }
    public string RoutePrefix { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? Version { get; set; }
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
