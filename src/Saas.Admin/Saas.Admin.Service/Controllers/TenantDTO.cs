using Saas.Admin.Service.Data;
using Saas.Admin.Service.Data.Models.OnBoarding;

namespace Saas.Admin.Service.Controllers;

public class TenantDTO
{
    /// <summary>
    /// For serialization don't use this
    /// </summary>
    public TenantDTO()
    {
        Name = string.Empty;
        Route = string.Empty;
        Version = string.Empty;
    }

    public TenantDTO(Tenant tenant)
    {
        Id = tenant.Guid;

        CreatedTime = DateTime.Now;//Guard.Argument(tenant.CreatedDate, nameof(tenant.CreatedDate)).NotNull();

        Name = Guard.Argument(tenant.Company, nameof(tenant.Company)).NotEmpty();
        Route = Guard.Argument(tenant.Route, nameof(tenant.Route)).NotEmpty();
        CreatorEmail = Guard.Argument(tenant.CreatedUser, nameof(tenant.CreatedUser)).NotEmpty();
        ProductTierId = tenant.ProductTierId;
        CategoryId = tenant.Industry;

        Version = tenant.ConcurrencyToken is not null 
            ? Convert.ToBase64String(tenant.ConcurrencyToken) 
            : null;
    }

    public Tenant ToTenant()
    {
        Tenant tenant = new Tenant()
        {
            Guid = Id,
            Company = Name,
            Route = Route,
            CreatedUser = CreatorEmail,
            ProductTierId = ProductTierId,
            Industry = CategoryId,
            ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null,
            CreatedDate = null,
        };
        return tenant;
    }


    public void CopyTo(Tenant target)
    {
        target.Company = Name;
        target.Route = Route;
        target.CreatedUser = CreatorEmail;
        target.Industry = CategoryId;
        target.ProductTierId = ProductTierId;
        target.ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int ProductTierId { get; set; }
    public int CategoryId { get; set; }
    public string CreatorEmail { get; set; } = string.Empty;
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
