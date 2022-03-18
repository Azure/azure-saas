namespace Saas.Admin.Service.Controllers;

public class NewTenantRequest
{
    public string Name { get; set; } = String.Empty;
    public string RoutePrefix { get; set; } = String.Empty;
    Guid OwnerId { get; set; }

    internal Tenant ToTenant()
    {
        Tenant tenant = new Tenant(this.Name, this.RoutePrefix)
        {
            IsCancelled = false,
            IsProvisioned = false,
            ConcurrencyToken = null,
            CreatedTime = null,
        };
        return tenant;
    }
}
