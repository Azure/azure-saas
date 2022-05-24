using Saas.Common.Interface;
using Saas.Common.Utility;
using System.Security.Claims;
using System.Web;

namespace Saas.SignupAdministration.Web.Models
{
    public class ApplicationUser : ClaimsIdentity, IApplicationUser
    {
        private static ClaimsIdentity Identity
        {
            get
            {
                return AppHttpContext.Current.User.Identity as ClaimsIdentity;
            }
        }

        public string EmailAddress
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.EmailAddressClaimType);
                string emailAddress = claim?.Value;

                return (!string.IsNullOrWhiteSpace(emailAddress) || RegexUtilities.IsValidEmail(emailAddress)) ? emailAddress : (emailAddress == null) ? throw new ArgumentNullException("EmailAddress") : throw new ArgumentException("The email addres must be in a valid format", "EmailAddress");
            }
        }

        public Guid NameIdentifier
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.NameIdentifierClaimType);

                return (Guid.TryParse(claim?.Value, out Guid nameIdentifier)) ? nameIdentifier : throw new ArgumentNullException("NameIdentifier");
            }
        }

        public string AuthenticationClassReference
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.AuthenticationClassReferenceClaimType);

                return claim?.Value;
            }
        }

        public DateTime AuthenticationTime
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.AuthenticationTimeClaimType);

                bool success = long.TryParse(claim?.Value, out long ticks);

                return new DateTime((success) ? ticks : 0);
            }
        }

        public long AuthenticationTimeTicks
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.AuthenticationTimeClaimType);

                bool success = long.TryParse(claim?.Value, out long ticks);

                return (success) ? ticks : 0;
            }
        }

        public string GivenName
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.GivenNamClaimType);

                return claim?.Value;
            }
        }

        public string Surname
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.SurnameClaimType);

                return claim?.Value;
            }
        }

        public Guid TenantId
        {
            get
            {
                Claim claim = Identity?.FindFirst(SR.TenantIdClaimType);

                return (Guid.TryParse(claim?.Value, out Guid tenantId)) ? tenantId : throw new ArgumentNullException("TenantId");
            }
        }
    }
}
