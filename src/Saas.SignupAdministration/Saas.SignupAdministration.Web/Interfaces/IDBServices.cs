namespace Saas.SignupAdministration.Web.Interfaces;

public interface IDBServices
{
    Task<bool> isUserRegistered(string email);
}
