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
        // Insert any other custom certificate validation logic here


        // Do not check your certificate thumbprint into your git repository.
        // Another option would be to load in your certificate thumbprint from azure keyvault.
        var expectedCertificateThumbPrint = _config.GetValue<string>("SelfSignedCertThumbprint");

        return clientCertificate.Thumbprint == expectedCertificateThumbPrint;
    }
}
