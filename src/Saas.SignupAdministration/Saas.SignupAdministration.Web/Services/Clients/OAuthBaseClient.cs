using Saas.Identity.Model;
using System.Net.Http;
using System.Threading;

namespace Saas.SignupAdministration.Web.Services;

// Create base client for Nswag generated clients that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    private readonly ITokenAcquisition _tokenAcquisition;

    private readonly IEnumerable<string> _scopes;

    public OAuthBaseClient(
        ITokenAcquisition tokenAcquisition,
        IOptions<SaaSAppScopeOptions> scopes)
    {
        _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
        _scopes = scopes.Value.Scopes ?? throw new ArgumentNullException($"Scopes must be defined.");
    }

    protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
    {
        HttpRequestMessage msg = new();
        var bearerToken = await GetAccessToken();
        msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        return await Task.FromResult(msg);
    }

    private async Task<string> GetAccessToken()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        return accessToken;
    }
}
