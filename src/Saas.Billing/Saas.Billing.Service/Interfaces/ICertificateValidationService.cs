using System.Security.Cryptography.X509Certificates;

namespace Saas.Billing.Service.Interfaces;

public interface ICertificateValidationService
{
    public bool ValidateCertificate(X509Certificate2 clientCertificate);
}
