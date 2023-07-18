
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Saas.Identity.Crypto;
using Saas.Identity.Interface;
using Saas.Identity.Provider;
using Saas.Shared.Interface;
using Saas.Shared.Options;

namespace Saas.Identity.Extensions;
public static partial class SaasIdentityConfigurationBuilderExtensions
{

    public static SaasApiClientCredentialBuilder<TProvider, TOptions> AddSaasApiCertificateClientCredentials<TProvider, TOptions>(
        this IServiceCollection services, 
        IEnumerable<string>? scopes = default)
        where TProvider : ISaasApi
        where TOptions : AzureAdB2CBase
    {

        services.AddMemoryCache();
        services.AddSingleton<IPublicX509CertificateDetailProvider, PublicX509CertificateDetailProvider>();
        services.AddSingleton<IClientAssertionSigningProvider, ClientAssertionSigningProvider>();
        services.AddScoped<SaasApiAuthenticationProvider<TProvider, TOptions>>();

        switch (scopes)
        {
            case null when (typeof(TProvider).Equals(typeof(ISaasMicrosoftGraphApi))):
                {
                    services.Configure<SaasApiScopeOptions<TProvider>>(options
                        => options.Scopes = new[] { "https://graph.microsoft.com/.default" });
                    break;
                }
            case null:
                throw new ArgumentException("Client Credentials scopes must be defined in the form: <Some App Id uri>/.default");
            default:
                {
                    services.Configure<SaasApiScopeOptions<TProvider>>(options
                        => options.Scopes = scopes.ToArray());
                    break;
                }
        }

        return new SaasApiClientCredentialBuilder<TProvider, TOptions>(services);
    }
}

public class SaasApiClientCredentialBuilder<TProvider, TOptions>
    where TProvider : ISaasApi
    where TOptions : AzureAdB2CBase
{
    private readonly IServiceCollection _services;

    public SaasApiClientCredentialBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IServiceCollection AddMicrosoftGraphAuthenticationProvider()
    {
        if (typeof(TProvider).Equals(typeof(ISaasMicrosoftGraphApi)))
        {
            _services.AddScoped<IAuthenticationProvider, SaasGraphClientCredentialsProvider<TOptions>>();
        }
        else
        {
            throw new ArgumentException($"Provider type is of {typeof(TProvider)}, to add Microsoft Graph Autentication provider it must be of type {typeof(ISaasMicrosoftGraphApi)}");
        }
        
        return _services;
    }
}
