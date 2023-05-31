namespace Saas.Admin.Service.Services;

public interface IAdminGraphServices
{
    public Task<CUser> GetUser(string userEmail);
}
