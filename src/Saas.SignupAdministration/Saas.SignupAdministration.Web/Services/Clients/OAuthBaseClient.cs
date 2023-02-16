using Saas.Identity.Model;
using Saas.SignupAdministration.Web.Models;
using System.Net.Http;
using System.Threading;

namespace Saas.SignupAdministration.Web.Services;

// Create base client for Nswag generated clients that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    //public string BearerToken { get; private set; } = string.Empty;
    private readonly ITokenAcquisition _tokenAcquisition;

    private readonly IEnumerable<string>? _scopes;

    //private readonly AppSettings _appSettings;

    public OAuthBaseClient(
        ITokenAcquisition tokenAcquisition,
        IOptions<SaaSAppScopeOptions> scopes)
    {
        _tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
        _scopes = scopes.Value.Scopes;
        //_appSettings = appSettings.Value;
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
        // var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_appSettings.AdminServiceScopes.Split(" "));
        return accessToken;
    }
}
