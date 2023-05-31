using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Services;
using Saas.Shared.Options;

namespace Saas.Admin.Service.Services;

public class AdminGraphServices :  IAdminGraphServices
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly AzureB2CPermissionsApiOptions _permissionOptions;
    public AdminGraphServices
        (
        IOptions<AzureB2CPermissionsApiOptions> permissionApiOptions,
        IGraphApiClientFactory graphClientFactory

        )
    {
        _graphServiceClient = graphClientFactory.Create();
        _permissionOptions = permissionApiOptions.Value;
    }


    public async Task<CUser> GetUser(string userEmail)
    {
        try
        {
            var response = await _graphServiceClient.Users.Request()
              .Filter($"identities/any(id: id/issuer eq '{_permissionOptions.Domain}' and id/issuerAssignedId eq '{userEmail}')")
                .Select("id, identitied, displayName, givenName, surname, mail")
                .GetAsync();

            CUser user = ToUserObjects(response).First();

            return user;
        
        }
        catch (Exception ex)
        {
           
            throw ex;
        }
    }

    private static IEnumerable<CUser> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers) =>
       graphUsers.Select(graphUser => new CUser()
       {
           UserId = graphUser.Id,
           DisplayName = graphUser.DisplayName,
           GivenName = graphUser.GivenName,
           Surname = graphUser.Surname,
           Mail = graphUser.Mail
           
       });
}

public class CUser
{
    public string UserId { get; set; }

    public string DisplayName { get; set; }

    public string GivenName { get; set; }


    public string Surname { get; set; }
    public string Mail { get; set; }
}
