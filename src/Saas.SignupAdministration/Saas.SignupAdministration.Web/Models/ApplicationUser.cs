using Saas.Application.Web;
using Saas.SignupAdministration.Web.Utilities;
using System.Security.Claims;

namespace Saas.SignupAdministration.Web.Models;

public class ApplicationUser : ClaimsIdentity, IApplicationUser
{
    private static ClaimsIdentity? Identity => AppHttpContext.Current?.User.Identity as ClaimsIdentity;

    public string EmailAddress
    {
        get
        {
            var claim = Identity?.FindFirst(SR.EmailAddressClaimType);
            string emailAddress = claim?.Value ?? string.Empty;

            return (!string.IsNullOrWhiteSpace(emailAddress) || RegexUtilities.IsValidEmail(emailAddress)) ? emailAddress : (emailAddress == null) ? throw new ArgumentNullException("EmailAddress") : throw new ArgumentException("The email addres must be in a valid format", "EmailAddress");
        }
    }

    public string Telephone
    {
        get
        {
            var claim = Identity?.FindFirst(SR.TelephoneClaimType);
            string telephone = claim?.Value ?? string.Empty;

            return telephone;

        }
    }

    public string Country
    {
        get
        {
            var claim = Identity?.FindFirst("country");
            string country = claim?.Value ?? string.Empty;

            return country;
        }
    }

    public string Industry
    {
        get
        {
            var claim = Identity?.FindFirst("industry");
            string industry = claim?.Value ?? string.Empty;

            return industry;
        }
    }

    public Guid NameIdentifier
    {
        get
        {
            var claim = Identity?.FindFirst(SR.NameIdentifierClaimType);

            return (Guid.TryParse(claim?.Value, out Guid nameIdentifier)) ? nameIdentifier : throw new ArgumentNullException("NameIdentifier");
        }
    }

    public string AuthenticationClassReference
    {
        get
        {
            var claim = Identity?.FindFirst(SR.AuthenticationClassReferenceClaimType);

            return claim?.Value ?? string.Empty;
        }
    }

    public DateTime AuthenticationTime
    {
        get
        {
            var claim = Identity?.FindFirst(SR.AuthenticationTimeClaimType);

            bool success = long.TryParse(claim?.Value, out long ticks);

            return new DateTime((success) ? ticks : 0);
        }
    }

    public long AuthenticationTimeTicks
    {
        get
        {
            var claim = Identity?.FindFirst(SR.AuthenticationTimeClaimType);

            bool success = long.TryParse(claim?.Value, out long ticks);

            return (success) ? ticks : 0;
        }
    }

    public string FullName
    {
        get
        {
            var claim = Identity?.FindFirst("name");

            string name = claim?.Value ?? EmailAddress.Split('@').First();
            return  name;
        }
    }

    public string Surname
    {
        get
        {
            var claim = Identity?.FindFirst(SR.SurnameClaimType);

            return claim?.Value ?? string.Empty;
        }
    }

    public Guid TenantId
    {
        get
        {
            var claim = Identity?.FindFirst(SR.TenantIdClaimType);

            return (Guid.TryParse(claim?.Value, out Guid tenantId)) ? tenantId : throw new ArgumentNullException("TenantId");
        }
    }
}
