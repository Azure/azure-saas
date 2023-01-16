using Azure.Core;
using Azure.Identity;
using Saas.Permissions.Service.Interfaces;

namespace Saas.Permissions.Service.Services;

public class ProductionManagedIdentityKeyVaultService : IKeyVaultCredentialService
{
    private readonly TokenCredential _tokenCredential; 

    public ProductionManagedIdentityKeyVaultService(IConfiguration configuration)
    {
        var userManagedIdentityClientId = configuration.GetRequiredSection("UserAssignedManagedIdentityClientId")?.Value
            ?? throw new NullReferenceException("The Environment Variable 'UserAssignedManagedIdentityClientId' cannot be null. Check the App Service Configuration.");

        ManagedIdentityCredential credential = new(userManagedIdentityClientId);

        _tokenCredential = credential;
    }

    public TokenCredential GetCredential() => _tokenCredential;
}
