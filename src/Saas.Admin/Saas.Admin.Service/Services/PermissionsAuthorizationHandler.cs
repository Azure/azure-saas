using Microsoft.AspNetCore.Authorization;

namespace Saas.Admin.Service.Services;

public class PermissionsAuthorizationHandler : AuthorizationHandler<TenantStuff>
{
    public PermissionsAuthorizationHandler()
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantStuff requirement)
    {
        throw new NotImplementedException();
    }
}


public class TenantStuff : IAuthorizationRequirement
{

}