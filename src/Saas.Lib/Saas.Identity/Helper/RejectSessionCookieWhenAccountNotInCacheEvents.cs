using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace Saas.Identity.Helper;

// For more details please see: https://github.com/AzureAD/microsoft-identity-web/issues/13#issuecomment-878528492
public class RejectSessionCookieWhenAccountNotInCacheEvents : CookieAuthenticationEvents
{
    private readonly IEnumerable<string> _scopes;

    public RejectSessionCookieWhenAccountNotInCacheEvents(IEnumerable<string> scopes)
    {
        _scopes = scopes;
    }

    public async override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        try
        {
            var tokenAcquisition = context.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
            string token = await tokenAcquisition.GetAccessTokenForUserAsync(
                _scopes,
                user: context.Principal);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
           when (AccountDoesNotExitInTokenCache(ex))
        {
            context.RejectPrincipal();
        }
    }

    /// <summary>
    /// Is the exception thrown because there is no account in the token cache?
    /// </summary>
    /// <param name="ex">Exception thrown by <see cref="ITokenAcquisition"/>.GetTokenForXX methods.</param>
    /// <returns>A boolean telling if the exception was about not having an account in the cache</returns>
    private static bool AccountDoesNotExitInTokenCache(MicrosoftIdentityWebChallengeUserException ex)
        => ex.InnerException is MsalUiRequiredException msalUiRequiredException
                    && msalUiRequiredException.ErrorCode is "user_null";
}