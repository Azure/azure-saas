using Saas.Admin.Service.Data;
using Saas.Admin.Service.Data.Models.OnBoarding;

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
        Id = tenant?.Guid ?? Guid.Empty;
        Name = Guard.Argument(tenant?.Company, nameof(tenant.Company)).NotEmpty();
        Route = Guard.Argument(tenant?.Route, nameof(tenant.Route)).NotEmpty();
        Version = tenant?.ConcurrencyToken != null ? Convert.ToBase64String(tenant.ConcurrencyToken) : null;
    }

    public Tenant ToTenant()
    {
        Tenant tenant = new Tenant()
        {
            Guid = Id,
            Company = Name,
            Route = Route,
            ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null,
            CreatedDate = null,
        };
        return tenant;
    }


    public void CopyTo(Tenant target)
    {
        target.Company = Name;
        target.Route = Route;
        target.ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Version { get; set; }
}
