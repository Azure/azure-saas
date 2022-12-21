namespace ClientAssertionWithKeyVault.Interface;

public interface IKeyInfo
{
    string? KeyVaultUrl { get; }
    string? KeyVaultCertificateName { get; }
}
