namespace Saas.Billing.Service.Controllers;

public partial class SubscriptionDTO
{
    public string SubscriptionId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string ProductTierId { get; set; } = string.Empty;
    public System.DateTimeOffset CreatedDate { get; set; }
    public System.DateTimeOffset ServiceStartDate { get; set; }
    public System.DateTimeOffset ServiceEndDate { get; set; }
}
