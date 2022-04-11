using System.Net;
using System.Net.Mail;

namespace Saas.SignupAdministration.Web.Services
{
    public class Email : IEmail
    {
        private readonly EmailOptions _options;

        public Email(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public bool Send(string recipientAddress)
        {
           if(!int.TryParse(_options.Port, out int port))
            {
                throw new InvalidCastException($"The value of {port} could not be cast to an integer");
            }

            using SmtpClient client = new(_options.Host, port);

            // TODO: Should update this to use a secure string
            // These credentails are not secure and will be passed in plain text in this example
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);
            MailAddress fromAddress = new(_options.FromAddress);
            MailAddress toAddress = new(recipientAddress);
            MailMessage message = new(fromAddress, toAddress);

            message.Subject = _options.Subject;
            message.Body = _options.Body;

            try
            {
                client.Send(message);
            }
            catch
            {
                // TODO: Need to add robust error handling here
                return false;
            }
            finally
            {
                message.Dispose();
                client.Dispose();
            }

            return true;
        }
    }
}
