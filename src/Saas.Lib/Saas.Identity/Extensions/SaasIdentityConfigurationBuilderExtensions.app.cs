using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Saas.Shared.Options;

namespace Saas.Identity.Extensions;
public static partial class SaasIdentityConfigurationBuilderExtensions
{
    public static SaasWebAppClientCredentialBuilder AddSaasWebAppAuthentication(
        this IServiceCollection services,
        string configSectionName,
        ConfigurationManager configuration,
        IEnumerable<string> scopes)
    {        
        // Registerer scopes to the Options collection
        services.Configure<SaasAppScopeOptions>(saasAppScopeOptions => 
            saasAppScopeOptions.Scopes = scopes.ToArray());


        var authenticationBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
             .AddMicrosoftIdentityWebApp(options =>
             {
                 configuration.Bind(configSectionName, options);
             });

        return new SaasWebAppClientCredentialBuilder(services, authenticationBuilder, scopes);
    }

    public static SaasWebAppClientCredentialBuilder AddSaasWebAppAuthentication(
    this IServiceCollection services,
    IEnumerable<string> scopes,
    Action<MicrosoftIdentityOptions> configureMicrosoftIdentityOptions)
    {
        // Registerer scopes to the Options collection
        services.Configure<SaasAppScopeOptions>(saasAppScopeOptions =>
            saasAppScopeOptions.Scopes = scopes.ToArray());


        var authenticationBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(configureMicrosoftIdentityOptions,
            //start  enable insecure cookie transmission
            opts =>
            {
                opts.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                opts.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
            }
            //end
            );

        return new SaasWebAppClientCredentialBuilder(services, authenticationBuilder, scopes);
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

        public MicrosoftIdentityAppCallsWebApiAuthenticationBuilder SaaSAppCallDownstreamApi(IEnumerable<string>? scopes = default)
        {
            return _authenticationBuilder
                .EnableTokenAcquisitionToCallDownstreamApi(
                    options =>
                    {
                        // In case of wanting to make changes to the ConfidentialClientApplicationOptions
                    },
                    scopes ?? _scopes);
        }}
    }
