using Saas.Permissions.Api.Data;
using Saas.Permissions.Api.Interfaces;

namespace Saas.Permissions.Api.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly PermissionsContext _context;
        public PermissionsService(PermissionsContext permissionsContext)
        {
            _context = permissionsContext;
        }

        public async Task<ICollection<Permission>> GetPermissionsAsync(Guid userId)
        {
            return await _context.Permissions.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
