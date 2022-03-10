using Microsoft.AspNetCore.Mvc;

using Saas.Admin.Service.Data;

namespace Saas.Admin.Service.Services
{
    public interface ISubscriptionService
    {
        
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();

        Task<Subscription> GetSubscriptionAsync(Guid subscriptionId);

        Task<Subscription> AddSubscriptionAsync(Subscription subscription);

        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);

        Task DeleteSubscriptionAsync(Guid subscriptionId);

        Task<bool> SubscriptionExistsAsync(Guid subscriptionId);
    }
}
