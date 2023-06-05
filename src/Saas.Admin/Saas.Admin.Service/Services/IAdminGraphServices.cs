using Saas.SignupAdministration.Web.Models;

namespace Saas.Admin.Service.Services;

public interface IAdminGraphServices
{
    public Task<SadUser> GetUser(string userEmail);
}
