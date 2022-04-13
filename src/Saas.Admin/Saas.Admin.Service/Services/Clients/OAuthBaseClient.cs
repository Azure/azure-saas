namespace Saas.Admin.Service.Services;

// Create base client for Nswag generated clients that gets an access token using the passed in ITokenAcquisition interface.
public abstract class OAuthBaseClient
{
    public string BearerToken { get; private set; } = string.Empty;
    private readonly ITokenAcquisition _tokenAcquisition;

    private readonly PermissionsApiOptions _permissionsApiOptions;

    public OAuthBaseClient(ITokenAcquisition tokenAcquisition, IOptions<PermissionsApiOptions> permissionsApiOptions)
    {
        _permissionsApiOptions = permissionsApiOptions.Value;
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
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_permissionsApiOptions.Scopes);
        return accessToken;
    }
}
