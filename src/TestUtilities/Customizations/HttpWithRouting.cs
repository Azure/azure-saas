using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TestUtilities
{
    /// <summary>
    /// This customization will replace HttpContext with DefaultHttpContext and makes sure there is a routing feature
    /// To modify routing add "[Frozen]IRoutingFeature routing" as one of your function parameters.
    /// </summary>
    public class HttpWithRouting : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HttpContext>(composer =>
            {
                return composer.FromFactory<DefaultHttpContext, IRoutingFeature>((ctx, routing) =>
                {
                    ctx.Features[typeof(IRoutingFeature)] = routing;
                    return ctx;
                });
            });
        }
    }
}
