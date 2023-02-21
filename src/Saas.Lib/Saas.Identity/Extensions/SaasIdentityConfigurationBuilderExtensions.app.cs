
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Saas.Identity.Helper;
using Saas.Shared.Options;

namespace Saas.Identity.Extensions;
public static partial class SaasIdentityConfigurationBuilderExtensions
{
    public static SaasWebAppClientCredentialBuilder AddSaasWebAppAuthentication(
        this IServiceCollection services,
        string applicationIdUri,
        IEnumerable<string> scopes,
        ConfigurationManager configuration)
    {
        // Azure AD B2C requires scope config with a fully qualified url along with an identifier. To make configuring it more manageable and less
        // error prone, we store the names of the scopes separately from the application id uri and combine them when neded.
        var fullyQualifiedScopes = scopes.Select(scope => $"{applicationIdUri}/{scope}".Trim('/'));

        // Registerer scopes to the Options collection
        services.Configure<SaasAppScopeOptions>(saasAppScopeOptions => 
            saasAppScopeOptions.Scopes = fullyQualifiedScopes.ToArray());

        var authenticationBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                configuration.Bind(AzureB2CSaasAppOptions.SectionName, options);

            });

        // Managing the situation where the access token is not in cache.
        // For more details please see: https://github.com/AzureAD/microsoft-identity-web/issues/13
        services.Configure<CookieAuthenticationOptions>(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options => options.Events = new RejectSessionCookieWhenAccountNotInCacheEvents(fullyQualifiedScopes));

        return new SaasWebAppClientCredentialBuilder(services, authenticationBuilder, fullyQualifiedScopes);
    }

    public class SaasWebAppClientCredentialBuilder
    {
        private readonly IServiceCollection _services;
        private readonly MicrosoftIdentityWebAppAuthenticationBuilder _authenticationBuilder;
        private readonly IEnumerable<string> _scopes;

        public SaasWebAppClientCredentialBuilder(
            IServiceCollection services, 
            MicrosoftIdentityWebAppAuthenticationBuilder authenticationBuilder,
            IEnumerable<string> scopes)
        {
            _services = services;
            _authenticationBuilder= authenticationBuilder;
            _scopes= scopes;
        }

        public IHttpClientBuilder SaaSAppToCallDownstreamApiWithHttpClient<TIClient, TClient>(Action<IServiceProvider, HttpClient> action)
            where TIClient : class
            where TClient : class, TIClient
        {
            _authenticationBuilder
                .EnableTokenAcquisitionToCallDownstreamApi(
                    options =>
                    {
                        
                    },
                    _scopes)
                .AddInMemoryTokenCaches();

            return _services.AddHttpClient<TIClient, TClient>(action);
        }

        public IHttpClientBuilder SaaSAppToCallDownstreamApiWithHttpClient<TIClient, TClient>(string baseUrl)
            where TIClient : class
            where TClient: class, TIClient
        {
            Action<IServiceProvider, HttpClient> action = 
                (serviceProvider, client) => client.BaseAddress = new Uri(baseUrl);

            return SaaSAppToCallDownstreamApiWithHttpClient<TIClient, TClient>(action);
        }
    }
}
