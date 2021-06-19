using Microsoft.Identity.Client;
using Saas.Provider.Web.Models;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Services
{
    public class AzureAd
    {
        private readonly AppSettings _appSettings;

        private const string AuthorityFormat = "https://login.microsoftonline.com/{0}/v2.0";

        const string MSATenantId = "f4a840a3-b301-49d7-81dc-d598681eba1d";
        public static string clientId = "87e0a9e3-1faf-4cb3-abf6-6d9531de0658";
        public static string clientSecret = "k78~odcN_6TbVh5D~19_8Qkj~87trteArL";

        private const string MAIdentityScope = "https://mappz.onmicrosoft.com/ModernAppz.Api.Identity/.default";
        private const string MAOnboardingScope = "https://mappz.onmicrosoft.com/ModernAppz.Api.Onboarding/.default";

        public AzureAd(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(string service)
        {
            string scope = null;

            switch (service)
            {
                case "onboarding":
                    scope = MAOnboardingScope;
                    break;
                case "identity":
                    scope = MAIdentityScope;
                    break;
            }

            // Get a token for the Microsoft Graph. If this line throws an exception for any reason, we'll just let the exception be returned as a 500 response
            // to the caller, and show a generic error message to the user.
            IConfidentialClientApplication daemonClient;
            daemonClient = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(string.Format(AuthorityFormat, MSATenantId))
                .WithRedirectUri(_appSettings.RedirectUri)
                .WithClientSecret(clientSecret)
                .Build();

            AuthenticationResult authResult = await daemonClient.AcquireTokenForClient(new[] { scope })
                .ExecuteAsync();

            return authResult;
        }
    }
}