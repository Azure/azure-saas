using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;
using System.Text;
using Saas.Shared.Options;

namespace Saas.Permissions.Service.Services;

public class GraphAPIService(
    IOptions<AzureB2CPermissionsApiOptions> permissionApiOptions,
    IGraphApiClientFactory graphClientFactory,
    ILogger<GraphAPIService> logger) : IGraphAPIService
{
    private readonly ILogger _logger = logger;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(GraphAPIService)),
            "Client Assertion Signing Provider");

    private readonly GraphServiceClient _graphServiceClient = graphClientFactory.Create();
    private readonly AzureB2CPermissionsApiOptions _permissionOptions = permissionApiOptions.Value;

    public async Task<string[]> GetAppRolesAsync(ClaimsRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    public async Task<Models.User> GetUserByEmail(string userEmail)
    {
        try
        {
            var graphUsers = await _graphServiceClient.Users
                .GetAsync(requestionConfiguration =>
                {
                    requestionConfiguration.QueryParameters.Filter = $"identities/any(id: id/issuer eq '{_permissionOptions.Domain}' and id/issuerAssignedId eq '{userEmail}')";
                    requestionConfiguration.QueryParameters.Select = new string[] { "id, identities, displayName" };
                });

            if (graphUsers?.Value?.Count > 1)
            {
                throw new UserNotFoundException($"More than one user with the email {userEmail} exists in the Identity provider");
            }

            if (graphUsers?.Value?.Count == 0 || graphUsers?.Value is null)
            {
                throw new UserNotFoundException($"The user with the email {userEmail} was not found in the Identity Provider");
            }

            // Ok to just return first, because at this point we've verified we have exactly 1 user in the graphUsers object.
            return ToUserObjects(graphUsers.Value).First();
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    // Enriches the user object with data from Microsoft Graph. 
    public async Task<IEnumerable<Models.User>> GetUsersByIds(ICollection<Guid> userIds)
    {
        
        List<Models.User> userList = new();

        try
        {
            var graphUsers = await _graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = MakeUserFilter();
                });

            if (graphUsers?.Value is null)
            {
                return userList;
            }

            PageIterator<Microsoft.Graph.Models.User, UserCollectionResponse> pageIterator 
                = PageIterator<Microsoft.Graph.Models.User, UserCollectionResponse>
                    .CreatePageIterator(
                        _graphServiceClient,
                        graphUsers,  
                        (msg) => 
                        {
                            userList.Add(ToUserObject(msg));
                            return true;           
                        });

            await pageIterator.IterateAsync();

            return userList;
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }

        string MakeUserFilter ()
        {
            // Build graph query: "id in ('id1', 'id2')"
            // https://docs.microsoft.com/en-us/graph/aad-advanced-queries?tabs=csharp
            StringBuilder filter = new();
            filter.Append("id in (");
            filter.Append(string.Join(",", userIds.Select(id => $"'{id}'")));
            filter.Append(')');

            return filter.ToString();
        }
    }

    private async Task<ServicePrincipal?> GetServicePrincipalAsync(string clientId)
    {
        try
        {
            var servicePrincipal = await _graphServiceClient.ServicePrincipals
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"appId eq '{clientId}'";
                });

            return servicePrincipal?.Value?.SingleOrDefault();
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    private async Task<string[]> GetAppRoleAssignmentsAsync(ServicePrincipal servicePrincipal, string userId)
    {
        try
        {
            var userAppRoleAssignments = await _graphServiceClient.Users[userId].AppRoleAssignments
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.Equals($"resourceId eq {servicePrincipal.Id}");
                }) ?? throw new ArgumentException($"App role not found for \"{servicePrincipal.AppId}\".");

            var appRoleIds = userAppRoleAssignments?.Value?
                .Where(appRole => appRole is not null)
                .Where(appRole => appRole.AppRoleId is not null)
                .Select(appRole => appRole.AppRoleId);

            if (appRoleIds is null || !appRoleIds.Any())
            {
                throw new ArgumentException($"App role not found for \"{servicePrincipal.AppId}\".");
            }

            var appRoles = servicePrincipal.AppRoles?
                .Where(appRole => appRole?.Id is not null)
                .Where(appRole => appRoleIds.Contains(appRole.Id));

            if (appRoles is null || !appRoles.Any())
            {
                throw new ArgumentException($"App role not found for \"{servicePrincipal.AppId}\".");
            }   

            var roleClaimsArray = appRoles
                .Select(appRole => appRole.Value ?? string.Empty)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            return roleClaimsArray is not null && roleClaimsArray.Any()
                ? roleClaimsArray
                : throw new ArgumentException($"App role not found for \"{servicePrincipal.AppId}\".");
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    private static IEnumerable<Models.User> ToUserObjects(IEnumerable<Microsoft.Graph.Models.User> graphUsers) => 
        graphUsers.Select(graphUser => new Models.User()
        {
            UserId = graphUser.Id,
            DisplayName = graphUser.DisplayName
        });

    private static Models.User ToUserObject(Microsoft.Graph.Models.User graphUser) =>
        new()
        {
            UserId = graphUser.Id,
            DisplayName = graphUser.DisplayName
        };

}

