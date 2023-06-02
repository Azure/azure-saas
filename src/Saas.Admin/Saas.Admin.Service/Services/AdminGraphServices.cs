using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Services;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Models;

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


    public async Task<SadUser> GetUser(string userEmail)
    {
        try
        {
            var response = await _graphServiceClient.Users.Request()
              .Filter($"identities/any(id: id/issuer eq '{_permissionOptions.Domain}' and id/issuerAssignedId eq '{userEmail}')")
   .Select("id, identitied, displayName, givenName, surname, mail") //, createdDate, birthday, country

                .GetAsync();

            SadUser user = ToUserObjects(response).First();

            return user;
        
        }
        catch (Exception)
        {

            throw;
        }
    }

    private static IEnumerable<SadUser> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers) =>
       graphUsers.Select(graphUser => new SadUser()
       {

           FullNames = graphUser.DisplayName,
           Email = graphUser.Mail,
           Telephone = graphUser.MobilePhone,
           RegSource = "AZB2C",
           //DOB = new DateTime((DateTime.Now - (graphUser.Birthday??DateTimeOffset.UnixEpoch)).Ticks),
           Country = graphUser.Country,
           PrincipalUser = true,
           CreatedUser = "No",
          // CreatedDate = new DateOnly(graphUser.CreatedDateTime.Value.Year, graphUser.CreatedDateTime.Value.Month, graphUser.CreatedDateTime.Value.Day)
       });
}