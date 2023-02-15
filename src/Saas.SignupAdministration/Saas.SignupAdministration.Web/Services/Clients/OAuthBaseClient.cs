using Saas.SignupAdministration.Web.Models;
using System.Net.Http;
using System.Threading;

namespace Saas.SignupAdministration.Web.Services;

// Create base client for Nswag generated clients that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    //public string BearerToken { get; private set; } = string.Empty;
    private readonly ITokenAcquisition? _tokenAcquisition;

    //private readonly AppSettings _appSettings;

    public OAuthBaseClient(ITokenAcquisition tokenAcquisition)
    {
        tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
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
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "tenant.read"} );
        // var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_appSettings.AdminServiceScopes.Split(" "));
        return accessToken;
    }
}
