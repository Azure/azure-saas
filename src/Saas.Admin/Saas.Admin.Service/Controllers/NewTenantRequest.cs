using Saas.Admin.Service.Data;

namespace Saas.Admin.Service.Controllers;

public class NewTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string CreatorEmail { get; set; } = String.Empty;

    internal Tenant ToTenant()
    {
        Tenant tenant = new Tenant()
        {
            Name = Name,
            Route = Route,
            CreatorEmail = CreatorEmail,
            ConcurrencyToken = null,
            CreatedTime = null,
        };
        return tenant;
    }
}
