using System.Net;
using System.Net.Mail;

namespace Saas.SignupAdministration.Web.Services
{
    public class EmailOptions
    {
        public string EndPoint { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
