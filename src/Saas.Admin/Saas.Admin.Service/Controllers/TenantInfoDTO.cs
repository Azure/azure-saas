using Saas.Admin.Service.Data;

namespace Saas.Admin.Service.Controllers;

public class TenantInfoDTO
{
    /// <summary>
    /// For serialization don't use this
    /// </summary>
    public TenantInfoDTO()
    {
        Name = string.Empty;
        Route = string.Empty;
        Version = string.Empty;
    }

    public TenantInfoDTO(Tenant? tenant)
    {
        Id = tenant?.Id ?? Guid.Empty;
        Name = Guard.Argument(tenant?.Name, nameof(tenant.Name)).NotEmpty();
        Route = Guard.Argument(tenant?.Route, nameof(tenant.Route)).NotEmpty();
        Version = tenant?.ConcurrencyToken != null ? Convert.ToBase64String(tenant.ConcurrencyToken) : null;
    }

    public Tenant ToTenant()
    {
        Tenant tenant = new()
        {
            Id = Id,
            Name = Name,
            Route = Route,
            ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null,
            CreatedTime = null,
        };
        return tenant;
    }


    public void CopyTo(Tenant target)
    {
        target.Name = Name;
        target.Route = Route;
        target.ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Version { get; set; }
}
