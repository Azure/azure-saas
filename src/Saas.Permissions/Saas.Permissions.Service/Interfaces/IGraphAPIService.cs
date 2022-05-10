namespace Saas.Permissions.Service.Interfaces;

public interface IGraphAPIService
{
    public Task<ICollection<string>> GetAppRolesAsync(string userId);

}