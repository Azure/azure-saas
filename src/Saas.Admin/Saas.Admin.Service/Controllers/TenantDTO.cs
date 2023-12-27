using Saas.Admin.Service.Data;

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
        Id = tenant.Id;

        CreatedTime = Guard.Argument(tenant.CreatedTime, nameof(tenant.CreatedTime)).NotNull();

        Name = Guard.Argument(tenant.Name, nameof(tenant.Name)).NotEmpty();
        Route = Guard.Argument(tenant.Route, nameof(tenant.Route)).NotEmpty();
        CreatorEmail = Guard.Argument(tenant.CreatorEmail, nameof(tenant.CreatorEmail)).NotEmpty();
        ProductTierId = tenant.ProductTierId;
        CategoryId = tenant.CategoryId;

        Version = tenant.ConcurrencyToken is not null 
            ? Convert.ToBase64String(tenant.ConcurrencyToken) 
            : null;
    }

    public Tenant ToTenant()
    {
        Tenant tenant = new()
        {
            Id = Id,
            Name = Name,
            Route = Route,
            CreatorEmail = CreatorEmail,
            ProductTierId = ProductTierId,
            CategoryId = CategoryId,
            ConcurrencyToken = Version != null ? Convert.FromBase64String(Version) : null,
            CreatedTime = null,
        };
        return tenant;
    }


    public void CopyTo(Tenant target)
    {
        target.Name = Name;
        target.Route = Route;
        target.CreatorEmail = CreatorEmail;
        target.CategoryId = CategoryId;
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

public class TenantDTOPage(IEnumerable<TenantDTO> tenants, int totalCount, int startIndex)
{
    public IEnumerable<TenantDTO> Tenants { get; } = tenants;
    public int TotalCount { get; } = totalCount;
    public int StartIndex { get; } = startIndex;
}
