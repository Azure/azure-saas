using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Saas.Identity.Authorization.Model.Kind;
using Saas.Identity.Authorization.Option;
using Saas.Identity.Authorization.Requirement;

namespace Saas.Identity.Authorization.Handler;
public sealed class SaasTenantPermissionAuthorizationHandler : SaasPermissionAuthorizationHandlerBase<SaasTenantPermissionRequirement, TenantPermissionKind>
{
    public SaasTenantPermissionAuthorizationHandler(
        IHttpContextAccessor httpContextAccessor,
        IOptions<SaasAuthorizationOptions> saasAuthorizationOptions) : base(httpContextAccessor, saasAuthorizationOptions)
    {
    }
}
