using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sass.AspNetCore.Authorization.AuthHandlers
{
    public class RouteBasedRoleHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        private readonly string _routeValue;
        private readonly Func<RouteData, IEnumerable<string>, IEnumerable<string>> _roleFunc;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// This handler handles authorization for roles and will check for dynamically generated roles
        /// based on the route.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="contextAccessor"></param>
        /// <remarks>
        /// Default implementation looks for the value in the round and checks for
        /// roles in the form of {routeValue}.{role}
        /// 
        /// If the route is /subscriptions/{subscriptionId}/Users
        /// Authorize(Roles = "SubscriptionUser") will authorize roles SubscriptionUser AND {someId}.SubscriptionUser 
        /// where someId is extracted from the route.
        /// </remarks>
        public RouteBasedRoleHandler(IOptions<RouteBasedRoleHandlerOptions> options, IHttpContextAccessor contextAccessor)
        {
            _routeValue = options.Value.RouteValueName;
            _roleFunc = GenerateRolesFromRouteData;
            _contextAccessor = contextAccessor;
        }


        /// <summary>
        /// Use this to customize the handling.
        /// </summary>
        /// <param name="contextAccessor"></param>
        /// <param name="generateRolesFromRouteFunc"></param>
        public RouteBasedRoleHandler(IHttpContextAccessor contextAccessor, Func<RouteData, IEnumerable<string>, IEnumerable<string>> generateRolesFromRouteFunc)
        {
            _roleFunc = generateRolesFromRouteFunc;
            _routeValue = string.Empty;
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            HttpContext httpctx = _contextAccessor.HttpContext;

            if (httpctx != null)
            {
                IEnumerable<string> generatedRoles = this._roleFunc(httpctx.GetRouteData(), requirement.AllowedRoles);

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

        private IEnumerable<string> GenerateRolesFromRouteData(RouteData routeData, IEnumerable<string> allowedRoles)
        {

            string context = routeData.Values[this._routeValue] as string;

            if (context != null)
            {
                foreach (string role in allowedRoles)
                {
                    yield return String.Format("{0}.{1}", context, role);
                }
            }
        }
    }
}
