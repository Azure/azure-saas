//using Microsoft.AspNetCore.Authentication.OpenIdConnect;

//namespace Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Controllers;

//// https://damienbod.com/2022/05/16/using-multiple-azure-b2c-user-flows-from-asp-net-core/

//[AllowAnonymous]
//[Route("MicrosoftIdentity/[controller]/[action]")]
//public class AccountSignUpController : Controller
//{
//    [HttpGet("{scheme?}")]
//    public IActionResult SignUpPolicy(
//        [FromRoute] string scheme,
//        [FromQuery] string redirectUri)
//    {
//        scheme ??= OpenIdConnectDefaults.AuthenticationScheme;

//        string redirect = !string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri) 
//            ? redirectUri 
//            : Url.Content("~/");

//        AuthenticationProperties properties = new() { RedirectUri = redirect };

//        properties.Items[Constants.Policy] = "B2C_1A_SIGNUP_SIGNIN";
//        return Challenge(properties, scheme);
//    }
//}
