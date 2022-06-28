namespace Saas.Billing.Service.Controllers.DTOs;

public class NewSubscriptionRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string ProductTierId { get; set; } = string.Empty;
    public DateTime ServiceStartDate { get; set; }
}
