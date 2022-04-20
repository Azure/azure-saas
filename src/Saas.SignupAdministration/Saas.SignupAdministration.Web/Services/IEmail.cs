namespace Saas.SignupAdministration.Web.Services
{
    public interface IEmail
    {
        void Send(string recipientAddress);
    }
}