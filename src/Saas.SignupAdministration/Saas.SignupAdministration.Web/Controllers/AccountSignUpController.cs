using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Controllers;


[AllowAnonymous]
[Route("MicrosoftIdentity/[controller]/[action]")]
public class AccountSignUpController : Controller
{
    [HttpGet("{scheme?}")]
    public IActionResult SignUpPolicy(
        [FromRoute] string scheme,
        [FromQuery] string redirectUri)
    {
        scheme ??= OpenIdConnectDefaults.AuthenticationScheme;

        string redirect = !string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri) 
            ? redirectUri 
            : Url.Content("~/");

        AuthenticationProperties properties = new() { RedirectUri = redirect };

        properties.Items[Constants.Policy] = "B2C_1A_CLIENTCREDENTIALSFLOW";
        return Challenge(properties, scheme);
    }
}
