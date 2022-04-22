using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Saas.Permissions.Api.Interfaces;
using Saas.Permissions.Api.Models.AppSettings;
using System.Security.Cryptography.X509Certificates;
namespace Saas.Permissions.Api.Services;

public class CertificateValidationService : ICertificateValidationService
{
    private readonly AppSettings _appSettings;
    public CertificateValidationService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }
    public bool ValidateCertificate(X509Certificate2 clientCertificate)
    {
        // Insert any other custom certificate validation logic here


        // Do not check your certificate thumbprint into your git repository.
        // Another option would be to load in your certificate thumbprint from azure keyvault.
        var expectedCertificateThumbPrint = _appSettings.SSLCertThumbprint;

        return clientCertificate.Thumbprint == expectedCertificateThumbPrint;
    }
}
