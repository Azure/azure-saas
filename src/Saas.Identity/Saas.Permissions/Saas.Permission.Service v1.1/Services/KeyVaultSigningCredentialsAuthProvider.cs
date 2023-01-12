using Azure.Identity;
using ClientAssertionWithKeyVault.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Saas.Permissions.Service.Options;
using System.Net.Http.Headers;

namespace Saas.Permissions.Service.Services;

public class KeyVaultSigningCredentialsAuthProvider : IAuthenticationProvider
{
    private readonly MSGraphOptions _msGraphOptions;
    private readonly IClientAssertionSigningProvider _clientAssertionSigningProvider;
    private readonly IConfidentialClientApplication _msalClient;

    public KeyVaultSigningCredentialsAuthProvider(
        IOptions<MSGraphOptions> msGraphOptions,
        IOptions<PermissionApiOptions> permissionApiOptions,
        IClientAssertionSigningProvider clientAssertionSigningProvider)
    {
        _msGraphOptions = msGraphOptions.Value;

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

    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessTokenAsync());
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var scopes = _msGraphOptions?.Scopes?.Split(' ')
            ?? throw new NullReferenceException("Scopes cannot be null.");

        var result = await _msalClient
            .AcquireTokenForClient(scopes)
            .ExecuteAsync();

        return result.AccessToken;
    }
}
