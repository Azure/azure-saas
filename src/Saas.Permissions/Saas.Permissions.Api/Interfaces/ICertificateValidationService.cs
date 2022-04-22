namespace Saas.Permissions.Api.Interfaces;
using System.Security.Cryptography.X509Certificates;

public interface ICertificateValidationService
{
    public bool ValidateCertificate(X509Certificate2 clientCertificate);
}
