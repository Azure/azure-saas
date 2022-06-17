using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Net.Http;
using System.Threading;

namespace Saas.SignupAdministration.Web.Services;

// Create base client for Nswag generated clients that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    public string BearerToken { get; private set; } = string.Empty;
    private readonly ITokenAcquisition _tokenAcquisition;

    private readonly AppSettings _appSettings;

    public OAuthBaseClient(ITokenAcquisition tokenAcquisition, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _tokenAcquisition = tokenAcquisition;
    }

    protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
    {
        HttpRequestMessage msg = new HttpRequestMessage();
        BearerToken = await GetAccessToken();
        msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        return await Task.FromResult(msg);
    }

    private async Task<string> GetAccessToken()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_appSettings.AdminServiceScopes.Split(" ")); 
        return accessToken;
    }
}
