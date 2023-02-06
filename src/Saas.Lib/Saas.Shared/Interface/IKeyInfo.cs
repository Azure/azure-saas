namespace Saas.Interface;

public interface IKeyInfo
{
    string? KeyVaultUrl { get; }
    string? KeyVaultCertificateName { get; }
}
