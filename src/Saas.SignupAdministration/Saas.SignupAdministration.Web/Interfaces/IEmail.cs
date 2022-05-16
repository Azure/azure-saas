using System.Net.Http;

namespace Saas.SignupAdministration.Web.Services
{
    public interface IEmail
    {
        Task<HttpResponseMessage> SendAsync(string recipientAddress);
    }
}