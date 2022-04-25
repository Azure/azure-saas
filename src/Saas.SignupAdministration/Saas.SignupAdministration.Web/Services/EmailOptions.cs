using System.Net;
using System.Net.Mail;

namespace Saas.SignupAdministration.Web.Services
{
    public class EmailOptions
    {
        public string EndPoint { get; set; }    
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
