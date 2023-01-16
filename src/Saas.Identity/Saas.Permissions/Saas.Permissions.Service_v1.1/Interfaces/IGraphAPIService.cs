using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Interfaces;

public interface IGraphAPIService
{
    public Task<string[]> GetAppRolesAsync(ClaimsRequest request);
    public Task<IEnumerable<User>> GetUsersByIds(ICollection<Guid> userIds);
    public Task<User> GetUserByEmail(string userEmail);

}