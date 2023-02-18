using Azure.Core;
using Saas.Interface;

namespace Saas.Identity.Interface;

public interface IClientAssertionSigningProvider
{
    Task<string> GetClientAssertion(
        IKeyVaultInfo keyInfo,
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
