using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Options;
using System.Net.Http.Headers;
using ClientAssertionWithKeyVault.Interface;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Saas.Permissions.Service.Services;

public class KeyVaultSigningCredentialsAuthProvider : IAuthenticationProvider
{
    private readonly ILogger _logger;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(KeyVaultSigningCredentialsAuthProvider)),
            "Client Assertion Signing Provider");

    private readonly MSGraphOptions _msGraphOptions;
    private readonly IClientAssertionSigningProvider _clientAssertionSigningProvider;
    private readonly IConfidentialClientApplication _msalClient;

    public KeyVaultSigningCredentialsAuthProvider(
        IOptions<MSGraphOptions> msGraphOptions,
        IOptions<PermissionApiOptions> permissionApiOptions,
        IClientAssertionSigningProvider clientAssertionSigningProvider,
        IKeyVaultCredentialService credentialService,
        ILogger<KeyVaultSigningCredentialsAuthProvider> logger)
    {
        _logger= logger;
        _msGraphOptions = msGraphOptions.Value;
        _clientAssertionSigningProvider = clientAssertionSigningProvider;

        if (permissionApiOptions?.Value?.Certificates?[0] is null)
        {
            logger.LogError("Certificate cannot be null.");
            throw new NullReferenceException("Certificate cannot be null.");
        }

        _msalClient = ConfidentialClientApplicationBuilder
        .Create(permissionApiOptions.Value.ClientId)
        .WithAuthority(AzureCloudInstance.AzurePublic, permissionApiOptions.Value.TenantId)
        .WithClientAssertion(
            (AssertionRequestOptions options) =>
                _clientAssertionSigningProvider.GetClientAssertion(
                    permissionApiOptions.Value.Certificates[0],
                    options.TokenEndpoint,
                    options.ClientID,
                    credentialService.GetCredential(),
                    TimeSpan.FromMinutes(10)))
        .Build();
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
    {
        try
        {
            requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetAccessTokenAsync());
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
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
