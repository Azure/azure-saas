namespace Saas.Admin.Service.Services
{
    public class PermissionService : IPermissionService
    {
        public Task AddUserPermissionsToSubscriptionAsyc(Guid subscriptionId, string userId, string[] permissions)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetSubscriptionsForUserAsync(string userId, string? filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetSubscriptionUsersAsync(Guid subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserPermissionsForSubscriptionAsync(Guid subscriptionId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserPermissionsFromSubscriptionAsync(Guid subscriptionId, string userId, string[] permissions)
        {
            throw new NotImplementedException();
        }
    }
}
