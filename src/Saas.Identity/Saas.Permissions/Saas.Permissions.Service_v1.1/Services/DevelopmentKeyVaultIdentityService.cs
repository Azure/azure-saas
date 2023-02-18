//using Azure.Core;
//using Azure.Identity;
//using Saas.Permissions.Service.Interfaces;

//namespace Saas.Permissions.Service.Services;

//public class DevelopmentKeyVaultIdentityService : IKeyVaultCredentialService
//{
//    private readonly TokenCredential _tokenCredential;

//    public DevelopmentKeyVaultIdentityService()
//    {
//        DefaultAzureCredential credential = new();

//        _tokenCredential = credential;
//    }

//    public TokenCredential GetCredential() => _tokenCredential;
//}
