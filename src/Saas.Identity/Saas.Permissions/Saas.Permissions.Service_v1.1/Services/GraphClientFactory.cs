using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Shared.Options;
using Saas.Permissions.Service.Interfaces;

namespace Saas.Permissions.Service.Services;

public class GraphApiClientFactory : IGraphApiClientFactory
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly MSGraphOptions _msGraphOptions;
    private readonly HttpClient _httpClient;

    public GraphApiClientFactory(
        IOptions<MSGraphOptions> msGraphOptions,
        IAuthenticationProvider authenticationProvider,
        HttpClient httpClient)
    {
        _msGraphOptions = msGraphOptions.Value;
        _authenticationProvider = authenticationProvider;
        _httpClient = httpClient;
    }

    public GraphServiceClient Create() => 
            new(_httpClient, _msGraphOptions.BaseUrl)
            {
                AuthenticationProvider = _authenticationProvider
            };
    
}
