namespace Saas.SignupAdministration.Web.Services
{
    public interface IEmail
    {
        bool Send(string recipientAddress);
    }
}