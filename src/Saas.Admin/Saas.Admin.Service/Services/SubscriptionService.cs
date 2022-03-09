using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

using Saas.Admin.Service.Data;
using Saas.Admin.Service.Exceptions;

namespace Saas.Admin.Service.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionsContext _context;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;

        public SubscriptionService(SubscriptionsContext subscriptionContext, IPermissionService permissionService, ILogger<SubscriptionService> logger)
        {
            _context = subscriptionContext;
            _permissionService = permissionService;
            _logger = logger;
        }


        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscription.ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionAsync(Guid subscriptionId)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);

            if (subscription == null)
            {
                throw new ItemNotFoundExcepton("Subscription");
            }

            return subscription;
        }

        public async Task<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            _context.Subscription.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }


        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await SubscriptionExistsAsync(subscription.Id))
                {
                    throw new ItemNotFoundExcepton("Subscription");
                }
                else
                {
                    throw;
                }
            }
            return subscription;
        }


        public async Task DeleteSubscriptionAsync(Guid subscriptionId)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null)
            {
                throw new ItemNotFoundExcepton("Subscription");
            }

            _context.Subscription.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SubscriptionExistsAsync(Guid subscriptionId)
        {
            return await _context.Subscription.AnyAsync(e => e.Id == subscriptionId);
        }


        public async Task<IEnumerable<string>> GetSubscriptionUsersAsync(Guid subscriptionId)
        {
            IEnumerable<string> users = await _permissionService.GetSubsriptionUsersAsync(subscriptionId);
            return users;
        }

        public async Task<IEnumerable<string>> GetUserPermissionsForSubscriptionAsync(Guid subscriptionId, string userId)
        {
            IEnumerable<string> users = await _permissionService.GetUserPermissionsForSubscriptionAsync(subscriptionId, userId);
            return users;
        }


        public async Task AddUserPermissionsToSubscriptionAsync(Guid subscriptionId, string userId, string[] permissions)
        {
            await _permissionService.AddUserPermissionsToSubscriptionAsyc(subscriptionId, userId, permissions);
        }

        public async Task RemoveUserPermissionsFromSubscriptionAsync(Guid subscriptionId, string userId, string[] permissions)
        {
            await _permissionService.RemoveUserPermissionsFromSubscriptionAsync(subscriptionId, userId, permissions);
        }


        public async Task<IEnumerable<Guid>> GetSubscriptionsForUserAsync(string userId, string? filter = null)
        {
            IEnumerable<Guid> subscriptions = await _permissionService.GetSubscriptionsForUserAsync(userId, filter);
            return subscriptions;
        }
    }
}
