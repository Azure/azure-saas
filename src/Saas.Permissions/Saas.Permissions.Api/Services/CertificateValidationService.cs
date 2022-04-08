using Microsoft.Extensions.Configuration;
using Saas.Permissions.Api.Interfaces;
using System.Security.Cryptography.X509Certificates;
namespace Saas.Permissions.Api.Services;

public class CertificateValidationService : ICertificateValidationService
{
    private readonly IConfiguration _config;
    public CertificateValidationService(IConfiguration config)
    {
        _config = config;
    }
    public bool ValidateCertificate(X509Certificate2 clientCertificate)
    {
        // Don't hardcode passwords in production code.
        // Use a certificate thumbprint or Azure Key Vault.
        var expectedCertificateThumbPrint = _config.GetValue<string>("SelfSignedCertThumbprint");

        return clientCertificate.Thumbprint == expectedCertificateThumbPrint;
    }
}
