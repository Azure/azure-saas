using Azure.Core;

namespace Saas.Identity.Interface;
public interface IKeyVaultCredentialService
{
    TokenCredential GetCredential();
}
