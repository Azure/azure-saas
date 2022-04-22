using System.Collections.Generic;

namespace Saas.AspNetCore.Authorization.AuthHandlers
{
    public interface IRoleCustomizer
    {
        public IEnumerable<string> CustomizeRoles(IEnumerable<string> allowedRoles);
    }
}
