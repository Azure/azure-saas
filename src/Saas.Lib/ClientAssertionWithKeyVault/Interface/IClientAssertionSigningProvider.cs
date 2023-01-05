using Azure.Core;

namespace ClientAssertionWithKeyVault.Interface;

public interface IClientAssertionSigningProvider
{
    Task<string> GetClientAssertion(
        IKeyInfo keyInfo,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default);


    Task<string> GetClientAssertion(
        string keyVaultUri,
        string certKeyName,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default);
}
