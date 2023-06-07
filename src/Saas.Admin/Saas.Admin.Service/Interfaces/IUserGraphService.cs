using Saas.Admin.Service.Model;

namespace Saas.Admin.Service.Interfaces;

public interface IUserGraphService
{
    public Task<AppUser> GetUserInfoByEmail(string userEmail);
}
