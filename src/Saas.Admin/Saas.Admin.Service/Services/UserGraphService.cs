using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Admin.Service.Interfaces;
using Saas.Admin.Service.Model;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Shared.Options;

namespace Saas.Admin.Service.Services;

public class UserGraphService : IUserGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly AzureB2CPermissionsApiOptions _permissionOptions;

    public UserGraphService(
        IOptions<AzureB2CPermissionsApiOptions> permissionApiOptions,
        IGraphApiClientFactory graphClientFactory)
    {
        _permissionOptions = permissionApiOptions.Value;
        _graphServiceClient = graphClientFactory.Create();
    }
    public async Task<AppUser> GetUserInfoByEmail(string userEmail)
    {
        try
        {
            var graphUsers = await _graphServiceClient.Users
        .Request()
        .Filter($"identities/any(id: id/issuer eq '{_permissionOptions.Domain}' and id/issuerAssignedId eq '{userEmail}')")
        .Select("id, identitied, displayName")
        .GetAsync();

            if (graphUsers.Count > 1)
            {
                throw new UserNotFoundException($"More than one user with the email {userEmail} exists in the Identity provider");
            }
            if (graphUsers.Count == 0)
            {
                throw new UserNotFoundException($"The user with the email {userEmail} was not found in the Identity Provider");
            }

            // Ok to just return first, because at this point we've verified we have exactly 1 user in the graphUsers object.
            return ToUserObjects(graphUsers).First();
        }
        catch (Exception ex)
        { 
            throw ex;
        }
    }

    private static IEnumerable<AppUser> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers) =>
     graphUsers.Select(graphUser => new Model.AppUser()
     {
         UserId = graphUser.Id,
         DisplayName = graphUser.DisplayName,
         GivenName = graphUser.GivenName,
         Surname = graphUser.Surname,
         Mail = graphUser.Mail

     });
}
