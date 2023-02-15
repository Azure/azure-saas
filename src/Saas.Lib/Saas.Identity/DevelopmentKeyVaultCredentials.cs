using Azure.Core;
using Azure.Identity;
using Saas.Identity.Interface;

namespace Saas.Identity;
public  class DevelopmentKeyVaultCredentials : IKeyVaultCredentialService
{
    private readonly TokenCredential _tokenCredential;

    public DevelopmentKeyVaultCredentials()
    {
        DefaultAzureCredential credential = new();

        _tokenCredential = credential;
    }

    public TokenCredential GetCredential() => _tokenCredential;
}
