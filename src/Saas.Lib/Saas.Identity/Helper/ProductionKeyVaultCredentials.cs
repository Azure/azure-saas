using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Saas.Identity.Interface;

namespace Saas.Identity.Helper;
public class ProductionKeyVaultCredentials : IKeyVaultCredentialService
{
    private readonly TokenCredential _tokenCredential;

    public ProductionKeyVaultCredentials(IConfiguration configuration)
    {
        var userManagedIdentityClientId = configuration.GetRequiredSection("UserAssignedManagedIdentityClientId")?.Value
            ?? throw new NullReferenceException("The Environment Variable 'UserAssignedManagedIdentityClientId' cannot be null. Check the App Service Configuration.");

        ManagedIdentityCredential credential = new(userManagedIdentityClientId);

        _tokenCredential = credential;
    }

    public TokenCredential GetCredential() => _tokenCredential;
}
