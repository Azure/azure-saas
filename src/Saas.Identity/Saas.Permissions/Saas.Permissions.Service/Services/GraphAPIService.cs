﻿using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;
using Saas.Permissions.Service.Models.AppSettings;
using System.Text;

namespace Saas.Permissions.Service.Services;

public class GraphAPIService : IGraphAPIService
{
    private readonly GraphServiceClient _graphServiceClient;

    public GraphAPIService(IOptions<AzureADB2COptions> options)
    {

        ClientSecretCredential clientSecretCredential = new ClientSecretCredential(
           options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);


        _graphServiceClient = new GraphServiceClient(clientSecretCredential);


    }
    public async Task<string[]> GetAppRolesAsync(ClaimsRequest request)
    {
        ServicePrincipal? servicePrincipal = await GetServicePrincipalAsync(request.ClientId);

        if (servicePrincipal == null)
        {
            throw new ArgumentException($"App role not found for \"{request.ClientId}\".");
        }

        return await GetAppRoleAssignmentsAsync(servicePrincipal, request.ObjectId.ToString());
    }

    // Enriches the user object with data from Microsoft Graph. 
    public async Task<IEnumerable<Models.User>> GetUsersByIds(ICollection<Guid> userIds)
    {
        // Build graph query: "id in ('id1', 'id2')"
        // https://docs.microsoft.com/en-us/graph/aad-advanced-queries?tabs=csharp
        StringBuilder filter = new StringBuilder();
        filter.Append("id in (");
        filter.Append(string.Join(",", userIds.Select(id => $"'{id}'")));
        filter.Append(")");

        List<Models.User> userList = new List<Models.User>();


        var graphUsers = await _graphServiceClient.Users
        .Request()
        .Filter(filter.ToString())
        // Selects certain properties from the User object : https://docs.microsoft.com/en-us/graph/api/resources/user?view=graph-rest-1.0#properties
        // Add any additional fields here and then select them into the user model in ToUserObjects below
        .Select("id, displayName")
        .GetAsync();
        userList.AddRange(ToUserObjects(graphUsers));

        while(graphUsers.NextPageRequest != null)
        {
            graphUsers = await graphUsers.NextPageRequest.GetAsync();
            userList.AddRange(ToUserObjects(graphUsers));
        };

        return userList;
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
        var userAppRoleAssignments = await _graphServiceClient.Users[userId].AppRoleAssignments.Request().Filter($"resourceId eq {servicePrincipal.Id}").GetAsync();
        var appRoleIds = userAppRoleAssignments.Select(a => a.AppRoleId).ToArray();
        var appRoles = servicePrincipal.AppRoles.Where(a => appRoleIds.Contains(a.Id)).Select(a => a.Value).ToArray();
        return appRoles;
    }

    private IEnumerable<Models.User> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers)
    {
        return graphUsers.Select(graphUser => new Models.User()
        {
            UserId = graphUser.Id,
            DisplayName = graphUser.DisplayName
        });
    }

}

