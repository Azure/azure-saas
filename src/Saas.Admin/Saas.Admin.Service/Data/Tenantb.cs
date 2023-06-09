namespace Saas.Admin.Service.Data;

public class Tenantb
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int ProductTierId { get; set; }
    public int CategoryId { get; set; }
    public string CreatorEmail { get; set; } = string.Empty;
    public DateTime? CreatedTime { get; set; }
    [Timestamp]
    public byte[]? ConcurrencyToken { get; set; }
}
