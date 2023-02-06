using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Saas.AspNetCore.Authorization.AuthHandlers;

public class RouteBasedRoleCusomizer : IRoleCustomizer
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public RouteBasedRoleCusomizer(IHttpContextAccessor httpContextAccessor, string routeName, bool includeOriginals = false)
    {
        _httpContextAccessor = httpContextAccessor;
        IncludeOriginals = includeOriginals;
        RouteName = routeName;
    }

    public bool IncludeOriginals { get; internal set; }
    public string RouteName { get; protected set; }

    public IEnumerable<string> CustomizeRoles(IEnumerable<string> allowedRoles)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;

        string context = httpContext.GetRouteValue(RouteName) as string
            ?? throw new NullReferenceException("Routing name cannot be null");

        if (context is not null && allowedRoles is not null)
        {
            foreach (string role in allowedRoles)
            {
                yield return string.Format("{0}.{1}", context, role);
            }
            if (IncludeOriginals)
            {
                foreach (string role in allowedRoles)
                {
                    yield return role;
                }
            }
        }
    }
}
