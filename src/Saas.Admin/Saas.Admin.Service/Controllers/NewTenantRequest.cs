namespace Saas.Admin.Service.Controllers;

public class NewTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string RoutePrefix { get; set; } = string.Empty;
    private Guid OwnerId { get; set; }

    internal Tenant ToTenant()
    {
        Tenant tenant = new Tenant(Name, RoutePrefix)
        {
            IsCancelled = false,
            IsProvisioned = false,
            ConcurrencyToken = null,
            CreatedTime = null,
        };
        return tenant;
    }
}
