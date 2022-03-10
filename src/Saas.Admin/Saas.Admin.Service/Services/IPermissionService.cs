namespace Saas.Admin.Service.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<string>> GetSubscriptionUsersAsync(Guid subscriptionId);
        Task<IEnumerable<string>> GetUserPermissionsForSubscriptionAsync(Guid subscriptionId, string userId);
        Task AddUserPermissionsToSubscriptionAsyc(Guid subscriptionId, string userId, string[] permissions);
        Task RemoveUserPermissionsFromSubscriptionAsync(Guid subscriptionId, string userId, string[] permissions);
        Task<IEnumerable<Guid>> GetSubscriptionsForUserAsync(string userId, string? filter);
    }
}
