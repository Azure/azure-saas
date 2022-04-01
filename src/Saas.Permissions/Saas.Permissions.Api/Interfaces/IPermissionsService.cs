using Saas.Permissions.Api.Data;

namespace Saas.Permissions.Api.Interfaces
{
    public interface IPermissionsService
    {
        public Task<ICollection<Permission>> GetPermissionsAsync(Guid userId);
    }
}
