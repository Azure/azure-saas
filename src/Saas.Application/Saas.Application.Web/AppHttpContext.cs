using Microsoft.AspNetCore.Http;
using System;

namespace Saas.Application.Web;

public static class AppHttpContext
{
    static IServiceProvider services = null;

    /// <summary>
    /// Provides static access to the framework's services provider
    /// </summary>
    public static IServiceProvider Services
    {
        get 
        { 
            return services;
        }
        set
        {
            if (services != null)
            {
                throw new Exception("Can't set once a value has already been set.");
            }

            services = value;
        }
    }

    /// <summary>
    /// Provides static access to the current HttpContext
    /// </summary>
    public static HttpContext Current
    {
        get
        {
            IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

            return httpContextAccessor?.HttpContext;
        }
    }

}
