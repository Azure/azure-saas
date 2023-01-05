using Azure.Identity;
using ClientAssertionWithKeyVault.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Saas.Permissions.Service.Options;
using System.Net.Http.Headers;

namespace Saas.Permissions.Service.Services;

public class CertificateCredentialsAuthProvider : IAuthenticationProvider
{
    private readonly IClientAssertionSigningProvider _clientAssertionSigningProvider;
    private readonly IConfidentialClientApplication _msalClient;

    public CertificateCredentialsAuthProvider(
        IOptions<PermissionApiOptions> permissionApiOptions,
        IClientAssertionSigningProvider clientAssertionSigningProvider)
    {
        _clientAssertionSigningProvider = clientAssertionSigningProvider;

        DefaultAzureCredential credential = new();

        if (permissionApiOptions?.Value?.Certificates?[0] is null)
        {
            throw new NullReferenceException("Certificate cannot be null.");
        }

        _msalClient = ConfidentialClientApplicationBuilder
        .Create(permissionApiOptions.Value.ClientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, permissionApiOptions.Value.TenantId)
        .WithClientAssertion((AssertionRequestOptions options) =>
                _clientAssertionSigningProvider.GetClientAssertion(
                permissionApiOptions.Value.Certificates[0],
                options.TokenEndpoint,
                options.ClientID,
                credential,
                TimeSpan.FromMinutes(10)))
        .Build();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var result = await _msalClient.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync();

        return result.AccessToken;
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessTokenAsync());
    }
}
