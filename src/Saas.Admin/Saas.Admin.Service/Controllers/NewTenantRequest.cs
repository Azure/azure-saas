using Saas.Admin.Service.Data;

namespace Saas.Admin.Service.Controllers;

public class NewTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string RoutePrefix { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }

    internal Tenant ToTenant()
    {
        Tenant tenant = new Tenant()
        {
            ConcurrencyToken = null,
            CreatedTime = null,
        };
        return tenant;
    }
}
