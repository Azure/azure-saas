using Saas.Billing.Service.Controllers;
using Saas.Billing.Service.Controllers.DTOs;
using Saas.Billing.Service.Interfaces;

namespace Saas.Billing.Service.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ILogger _logger;

    public SubscriptionService(ILogger<SubscriptionService> logger)
    {
        _logger = logger;
    }

    //TODO (SaaS): Implement your payment processor or integrate with another service
    public SubscriptionDTO AddSubscriptionAsync(NewSubscriptionRequest newTenantRequest)
    {
        SubscriptionDTO returnValue = new SubscriptionDTO()
        {
            SubscriptionId = new Guid().ToString(),
            CustomerId = newTenantRequest.CustomerId,
            ProductTierId = newTenantRequest.ProductTierId,
            CreatedDate = DateTime.Now,
            ServiceStartDate = DateTime.Now,
        };
        return returnValue;
    }

}
