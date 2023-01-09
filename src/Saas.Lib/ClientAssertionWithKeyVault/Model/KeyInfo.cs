using ClientAssertionWithKeyVault.Interface;

namespace ClientAssertionWithKeyVault.Model;
internal record KeyInfo : IKeyInfo
{
    public KeyInfo(string keyVaultUrl, string keyVaultCertificateName)
    {
        KeyVaultUrl = keyVaultUrl;
        KeyVaultCertificateName = keyVaultCertificateName;
    }

    public string KeyVaultUrl { get; init; }

    public string KeyVaultCertificateName { get; init; }
}
