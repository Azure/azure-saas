using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.AspNetCore.Authorization.PolicyRequirements
{
    public class RouteBasedPolicyHandler : AuthorizationHandler<RouteBasedPolicyRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public RouteBasedPolicyHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RouteBasedPolicyRequirement requirement)
        {

            HttpContext httpctx = _contextAccessor.HttpContext;

            if (httpctx != null)
            {
                IEnumerable<string> generatedRoles = requirement.RoleGenerator(httpctx, requirement.AllowedRoles);

                if (generatedRoles != null)
                {
                    if (generatedRoles.Any(r => httpctx.User.IsInRole(r)))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
