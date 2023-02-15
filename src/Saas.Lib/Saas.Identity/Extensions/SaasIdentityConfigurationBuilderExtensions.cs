
using Microsoft.Extensions.DependencyInjection;
using Saas.Identity.Interface;
using Saas.Identity.Model;

namespace Saas.Identity.Extensions;
public static class SaasIdentityConfigurationBuilderExtensions
{
    public static void AddSaasWebApiAuthentication(
        this IServiceCollection services, 
        IEnumerable<string> scopes)
    {
        services.AddMemoryCache();
        services.AddSingleton<IPublicX509CertificateDetailProvider, PublicX509CertificateDetailProvider>();
        services.AddSingleton<IClientAssertionSigningProvider, ClientAssertionSigningProvider>();


        //services.Configure<SaaSAppScopeOptions>(option =>
        //{
        //    option = new() { Scopes = new[] {"1", "2"} };
        //});

        services.Configure<SaaSAppScopeOptions>(option => option.Scopes = scopes.ToArray());

        // return new SaasWebApiBuilder(services);
    }
}

public class SaasWebApiBuilder
{
    private readonly IServiceCollection _services;

    public SaasWebApiBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void AddScopes(IEnumerable<string> scopes)
    {
        _services.AddOptions<SaaSAppScopeOptions>().Configure(option => option = new() { Scopes = scopes.ToArray() });
    }
}
