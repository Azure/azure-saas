using Saas.Application.Web.Interfaces;
using Saas.Application.Web.Utilities;
using System.Security.Claims;

namespace Saas.Application.Web.Models;

public class ApplicationUser : ClaimsIdentity, IApplicationUser
{
    private static ClaimsIdentity? Identity => AppHttpContext.Current?.User?.Identity as ClaimsIdentity;

    public string EmailAddress
    {
        get
        {
            var claim = Identity?.FindFirst(SR.EmailAddressClaimType);
            string emailAddress = claim?.Value ?? string.Empty;

            return (!string.IsNullOrWhiteSpace(emailAddress) || RegexUtilities.IsValidEmail(emailAddress)) ? emailAddress : (emailAddress == null) ? throw new ArgumentNullException("EmailAddress") : throw new ArgumentException("The email addres must be in a valid format", "EmailAddress");
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

    public string GivenName
    {
        get
        {
            var claim = Identity?.FindFirst(SR.GivenNameClaimType);

            return claim?.Value ?? string.Empty;
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
