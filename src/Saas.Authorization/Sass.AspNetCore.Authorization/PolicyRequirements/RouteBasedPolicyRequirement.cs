using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using System;
using System.Collections.Generic;
using System.Text;

namespace Sass.AspNetCore.Authorization.PolicyRequirements
{
    public class RouteBasedPolicyRequirement : IAuthorizationRequirement
    {
        private readonly string _context;
        private readonly string[] _roles;
        private readonly bool _strict = false;

        public RouteBasedPolicyRequirement(string context, params string[] roles)
        {
            _context = context;
            _roles = roles;
            RoleGenerator = DefaultRoleGenerator;
        }

        public RouteBasedPolicyRequirement(string context, bool strict, params string[] roles)
        {
            _context = context;
            _roles = roles;
            _strict = strict;
            RoleGenerator = DefaultRoleGenerator;
        }

        public Func<HttpContext, IEnumerable<string>, IEnumerable<string>> RoleGenerator { get; set; }
        public IEnumerable<string> AllowedRoles { get => _roles; }

        private IEnumerable<string> DefaultRoleGenerator(HttpContext ctx, IEnumerable<string> allowedRoles)
        {
            RouteData routeData = ctx.GetRouteData();

            string context = routeData.Values[this._context] as string;

            if (context != null)
            {
                foreach (string role in allowedRoles)
                {
                    yield return String.Format("{0}.{1}", context, role);
                }
                foreach (string role in allowedRoles)
                {
                    yield return role;
                }
            }
        }
    }
}
