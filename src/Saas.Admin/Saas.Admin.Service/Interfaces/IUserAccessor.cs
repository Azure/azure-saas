using Saas.SignupAdministration.Web.Models;

namespace Saas.Admin.Service.Interfaces;

public interface IUserAccessor
{
    SadUser GetUser(SadUser sadUser);
    ISadUserDto GetUser();
}
