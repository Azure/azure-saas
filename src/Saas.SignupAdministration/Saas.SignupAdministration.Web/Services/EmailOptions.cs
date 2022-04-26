using System.Net;
using System.Net.Mail;

namespace Saas.SignupAdministration.Web.Services
{
    public class EmailOptions
    {
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Body { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
