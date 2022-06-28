using Saas.Billing.Service.Controllers;
using Saas.Billing.Service.Controllers.DTOs;

namespace Saas.Billing.Service.Interfaces;

public interface ISubscriptionService
{
    SubscriptionDTO AddSubscriptionAsync(NewSubscriptionRequest newTenantRequest);
}