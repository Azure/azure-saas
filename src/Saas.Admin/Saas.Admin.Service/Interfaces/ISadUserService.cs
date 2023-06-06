using Saas.SignupAdministration.Web.Models;

namespace Saas.Admin.Service.Interfaces;

public interface ISadUserService
{
    Task<SadUser> AddSadUser(SadUser sadUser, long userID);

}
