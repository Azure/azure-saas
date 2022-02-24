using Microsoft.Identity.Client;
using Saas.LandingSignup.Web.Models;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Services
{
    public class AzureAd
    {
        private readonly AppSettings _appSettings;

        private const string AuthorityFormat = SR.AzureAdAuthorityFormat;

        const string MSATenantId = "";

        public static string clientId = "";
        public static string clientSecret = "";

        private const string OnboardingScope = "";

        public AzureAd(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            // Get a token for the Microsoft Graph. If this line throws an exception for any reason, we'll just let the exception be returned as a 500 response
            // to the caller, and show a generic error message to the user.
            var daemonClient = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(string.Format(AuthorityFormat, MSATenantId))
                .WithRedirectUri(_appSettings.RedirectUri)
                .WithClientSecret(clientSecret)
                .Build();

            var authResult = await daemonClient.AcquireTokenForClient(new[] { OnboardingScope })
                .ExecuteAsync();

            return authResult;
        }
    }
}