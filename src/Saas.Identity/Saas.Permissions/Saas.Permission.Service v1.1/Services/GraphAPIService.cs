using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;
using Saas.Permissions.Service.Options;
using System.Text;

namespace Saas.Permissions.Service.Services;

public class GraphAPIService : IGraphAPIService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly PermissionApiOptions _permissionOptions;

    public GraphAPIService(
        IOptions<PermissionApiOptions> permissionApiOptions,
        IAuthenticationProvider authenticationProvider)
    {
        _graphServiceClient = new GraphServiceClient(authenticationProvider);
        _permissionOptions = permissionApiOptions.Value;
    }
    public async Task<string[]> GetAppRolesAsync(ClaimsRequest request)
    {
        if (request.ClientId is null)
        {
            throw new NullReferenceException("Client ID cannot be null.");
        }

        ServicePrincipal? servicePrincipal = await GetServicePrincipalAsync(request.ClientId);

        return servicePrincipal is null
            ? throw new ArgumentException($"App role not found for \"{request.ClientId}\".")
            : await GetAppRoleAssignmentsAsync(servicePrincipal, request.ObjectId.ToString());
    }

    public async Task<Models.User> GetUserByEmail(string userEmail)
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

    // Enriches the user object with data from Microsoft Graph. 
    public async Task<IEnumerable<Models.User>> GetUsersByIds(ICollection<Guid> userIds)
    {
        // Build graph query: "id in ('id1', 'id2')"
        // https://docs.microsoft.com/en-us/graph/aad-advanced-queries?tabs=csharp
        StringBuilder filter = new();
        filter.Append("id in (");
        filter.Append(string.Join(",", userIds.Select(id => $"'{id}'")));
        filter.Append(')');

        List<Models.User> userList = new();

        try
        {
            var graphUsers = await _graphServiceClient.Users
                .Request()
                .GetAsync();

            userList.AddRange(ToUserObjects(graphUsers));

            while (graphUsers.NextPageRequest is not null)
            {
                graphUsers = await graphUsers.NextPageRequest.GetAsync();
                userList.AddRange(ToUserObjects(graphUsers));
            };

            return userList;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<ServicePrincipal?> GetServicePrincipalAsync(string clientId)
    {
        var servicePrincipal = await _graphServiceClient.ServicePrincipals.Request()
            .Filter($"appId eq '{clientId}'")
            .GetAsync();
        
        return servicePrincipal.SingleOrDefault();
    }

    private async Task<string[]> GetAppRoleAssignmentsAsync(ServicePrincipal servicePrincipal, string userId)
    {
        var userAppRoleAssignments = await _graphServiceClient.Users[userId].AppRoleAssignments
            .Request()
            .Filter($"resourceId eq {servicePrincipal.Id}")
            .GetAsync();

        var appRoleIds = userAppRoleAssignments.Select(a => a.AppRoleId);
        
        var appRoles = servicePrincipal.AppRoles
            .Where(a => appRoleIds.Contains(a.Id))
            .Select(a => a.Value);
        
        return appRoles.ToArray();
    }

    private static IEnumerable<Models.User> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers) => 
        graphUsers.Select(graphUser => new Models.User()
        {
            UserId = graphUser.Id,
            DisplayName = graphUser.DisplayName
        });

}

