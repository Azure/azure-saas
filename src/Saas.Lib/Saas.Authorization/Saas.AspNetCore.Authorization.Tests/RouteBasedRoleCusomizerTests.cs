
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Xunit2;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using Saas.AspNetCore.Authorization.AuthHandlers;

using TestUtilities;

using Xunit;



namespace Saas.AspNetCore.Authorization.Tests
{
    public class RouteBasedRoleCusomizerTests
    {
        [Theory, AutoDataNSubstitute(typeof(HttpWithRouting))]
        public void Attribute_Behaves_As_Expected(IRoutingFeature wrongRouting, [Frozen]IRoutingFeature routing, HttpContext context, IHttpContextAccessor accessor)
        {
            Assert.NotSame(wrongRouting, routing);
            Assert.IsAssignableFrom<DefaultHttpContext>(context);
            Assert.Same(routing, context.Features[typeof(IRoutingFeature)]);
            Assert.Same(routing, accessor.HttpContext.Features[typeof(IRoutingFeature)]);
            routing.RouteData.Values.Add("foo", "bar");

            Assert.Equal("bar", accessor.HttpContext.GetRouteValue("foo"));

        }

        [Theory, AutoDataNSubstitute(typeof(HttpWithRouting))]
        public void Customize_ShouldReturnOriginals_WhenFlagSet([Frozen]IRoutingFeature routing, 
                                                                RouteBasedRoleCusomizer customizer, 
                                                                string[] allowedRoles, string routeValue)
        {
            customizer.IncludeOriginals = true;
            routing.RouteData.Values.Add(customizer.RouteName, routeValue);
            
            List<string>? result = customizer.CustomizeRoles(allowedRoles).ToList();

            //Should have double the roles
            Assert.Equal(allowedRoles.Length * 2, result.Count);

            //All original roles are in there.
            Assert.Equal(allowedRoles.Length, result.Intersect(allowedRoles).Count());
        }

        [Theory, AutoDataNSubstitute(typeof(HttpWithRouting))]
        public void Customize_ShouldReturnOriginals_WhenFlagNotSet([Frozen]IRoutingFeature routing, 
                                                                   RouteBasedRoleCusomizer customizer,
                                                                   string[] allowedRoles, string routeValue)
        {
            customizer.IncludeOriginals = false;
            routing.RouteData.Values.Add(customizer.RouteName, routeValue);
            List<string>? result = customizer.CustomizeRoles(allowedRoles).ToList();

            //Should have only updated roles
            Assert.Equal(allowedRoles.Length, result.Count);

            //No intersection between original roles and customized roles
            Assert.Empty(result.Intersect(allowedRoles));
        }

        [Theory, AutoDataNSubstitute(typeof(HttpWithRouting))]
        public void Customize_EmptyList_WhenRouteValueIsMissing([Frozen] IRoutingFeature routing, 
                                                                RouteBasedRoleCusomizer customizer, 
                                                                string[] allowedRoles, string routeValue)
        {
            customizer.IncludeOriginals = true;
            routing.RouteData.Values.Add("differentName", routeValue);
            
            List<string>? result = customizer.CustomizeRoles(allowedRoles).ToList();
            Assert.Empty(result);
        }

        [Theory, AutoDataNSubstitute(typeof(HttpWithRouting))]
        public void Customize_EmptyList_WhenAllowedRolesIsNull([Frozen] IRoutingFeature routing, 
                                                                RouteBasedRoleCusomizer customizer,
                                                                string routeValue)
        {
            customizer.IncludeOriginals = true;
            routing.RouteData.Values.Add("differentName", routeValue);
            routing.RouteData.Values.Add(customizer.RouteName, routeValue);

            List<string>? result = customizer.CustomizeRoles(null).ToList();
            Assert.Empty(result);
        }
    }
}