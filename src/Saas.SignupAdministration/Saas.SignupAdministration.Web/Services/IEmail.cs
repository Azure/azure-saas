namespace Saas.SignupAdministration.Web.Services
{
    public interface IEmail
    {
        void SendAsync(string recipientAddress);
    }
}