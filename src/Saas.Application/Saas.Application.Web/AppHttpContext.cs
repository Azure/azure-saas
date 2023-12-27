namespace Saas.Application.Web;

public static class AppHttpContext
{
    static IServiceProvider? services = null;

    /// <summary>
    /// Provides static access to the framework's services provider
    /// </summary>
    public static IServiceProvider? Services
    {
        get
        {
            return services;
        }
        set
        {
            if (services is not null)
            {
                throw new Exception("Can't set once a value has already been set.");
            }

            services = value;
        }
    }

    /// <summary>
    /// Provides static access to the current HttpContext
    /// </summary>
    public static HttpContext? Current
    {
        get
        {
            if(services is not null)
            {
                IHttpContextAccessor? httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

                return httpContextAccessor?.HttpContext;
            }
            else
            {
                return null;
            }
        }
    }

}
