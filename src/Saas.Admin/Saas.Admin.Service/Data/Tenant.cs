using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Admin.Service.Data;


public class Tenant
{
    public Tenant(string name, string routePrefix)
    {
        Name = Guard.Argument(name, nameof(name)).NotEmpty();
        RoutePrefix = Guard.Argument(routePrefix, nameof(routePrefix)).NotEmpty();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsProvisioned { get; set; }
    public string RoutePrefix { get; set; }
    public DateTime? CreatedTime { get; set; }

    [Timestamp]
    public byte[]? ConcurrencyToken { get; set; }
}
