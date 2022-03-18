namespace Saas.Admin.Service.Data;

public class Tenant
{
    public Tenant(string name, string createdBy, byte[] concurrencyToken)
    {
        Name = Guard.Argument(name, nameof(name)).NotEmpty();
        CreatedBy = Guard.Argument(createdBy, nameof(createdBy)).NotEmpty();
        ConcurrencyToken = concurrencyToken;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsProvisioned { get; set; }
    public Guid ApiKey { get; set; }
    public int CategoryId { get; set; }
    public int ProductId { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }

    [Timestamp]
    public byte[] ConcurrencyToken { get; set; }
}
