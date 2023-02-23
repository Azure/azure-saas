using Saas.Admin.Client;
using System.ComponentModel;

namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public partial class TenantViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Route { get; set; }
    public int ProductTierId { get; set; }
    [DisplayName("Service Plan")]
    public string ProductName { get; set; }
    public int CategoryId { get; set; }
    [DisplayName("Category")]
    public string CategoryName { get; set; }
    public string CreatorEmail { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Version { get; set; }

    public TenantViewModel(TenantDTO tenantDTO, IEnumerable<IdNameReferenceItem> categories, IEnumerable<IdNameReferenceItem> products)
    {
        Id = tenantDTO.Id;
        CreatedTime = tenantDTO.CreatedTime.LocalDateTime;
        Name = tenantDTO.Name;
        Route = tenantDTO.Route;
        CreatorEmail = tenantDTO.CreatorEmail;
        ProductTierId = tenantDTO.ProductTierId;
        ProductName = products.ElementAtOrDefault(tenantDTO.ProductTierId - 1)?.Name ?? "Free";
        CategoryId = tenantDTO.CategoryId;
        CategoryName = categories.ElementAtOrDefault(tenantDTO.CategoryId - 1)?.Name ?? "Unknown";
        Version = tenantDTO.Version;
    }
}
