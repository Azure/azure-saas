using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Saas.SignupAdministration.Web.Services
{
    public class Email : IEmail
    {
        private readonly EmailOptions _options;
        private readonly IHttpClientFactory _client;

        public Email(IOptions<EmailOptions> options, IHttpClientFactory client)
        {
            _options = options.Value;
            _client = client;
        }

        public async void Send(string recipientAddress)
        {

            var client = _client.CreateClient(_options.EndPoint);
            JSONEmail email = new JSONEmail();
            email.HTML = _options.Body;
            email.Subject = _options.Subject;
            email.EmailFrom = _options.FromAddress;
            email.EmailTo = recipientAddress;
            email.EmailToName = recipientAddress;

            var json = JsonSerializer.Serialize(email);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = client.PostAsync(_options.EndPoint, content).Result;
        }
    }
}