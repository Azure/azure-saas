using Saas.Interface;

namespace Saas.Identity.Model;
internal record KeyInfo : IKeyVaultInfo
{
    public KeyInfo(string keyVaultUrl, string keyVaultCertificateName)
    {
        KeyVaultUrl = keyVaultUrl;
        KeyVaultCertificateName = keyVaultCertificateName;
    }

    public string KeyVaultUrl { get; init; }

    public string KeyVaultCertificateName { get; init; }
}
