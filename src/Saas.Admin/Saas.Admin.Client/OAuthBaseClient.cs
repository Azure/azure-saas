using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Saas.Shared.Options;

namespace Saas.SignupAdministration.Web.Services;

// Base client for Nswag generated client that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    private readonly ITokenAcquisition _tokenAcquisition;

    private readonly IEnumerable<string> _scopes;

    public OAuthBaseClient(
        ITokenAcquisition tokenAcquisition,
        IOptions<SaasAppScopeOptions> scopes)
    {
        _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
        _scopes = scopes.Value.Scopes ?? throw new ArgumentNullException($"Scopes must be defined.");
    }

    protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken ct)
    {
        HttpRequestMessage msg = new();
        var bearerToken = await GetAccessToken(ct).ConfigureAwait(false);
        msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        return await Task.FromResult(msg);
    }

    private async Task<string> GetAccessToken(CancellationToken ct)
    {
        try
        {
            var accessToken = await _tokenAcquisition
                .GetAccessTokenForUserAsync(_scopes)
                .ConfigureAwait(false);

            return accessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            throw;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
