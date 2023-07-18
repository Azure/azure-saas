﻿using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;
using System.Text;
using Saas.Shared.Options;

namespace Saas.Permissions.Service.Services;

public class GraphAPIService : IGraphAPIService
{
    private readonly ILogger _logger;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(GraphAPIService)),
            "Client Assertion Signing Provider");

    private readonly GraphServiceClient _graphServiceClient;
    private readonly AzureB2CPermissionsApiOptions _permissionOptions;

    public GraphAPIService(
        IOptions<AzureB2CPermissionsApiOptions> permissionApiOptions,
        IGraphApiClientFactory graphClientFactory,
        ILogger<GraphAPIService> logger)
    {
        _logger= logger;
        _graphServiceClient = graphClientFactory.Create();
        _permissionOptions = permissionApiOptions.Value;
    }
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
            _logError(_logger, ex);
            throw;
        }
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
                .Filter(filter.ToString())
                .GetAsync();

            userList.AddRange(ToUserObjects(graphUsers));

            while (graphUsers.NextPageRequest is not null)
            {
                graphUsers = await graphUsers.NextPageRequest.GetAsync();
                userList.AddRange(ToUserObjects(graphUsers));
            };

            return userList;
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    private async Task<ServicePrincipal?> GetServicePrincipalAsync(string clientId)
    {
        try
        {
            var servicePrincipal = await _graphServiceClient.ServicePrincipals.Request()
                .Filter($"appId eq '{clientId}'")
                .GetAsync();

            return servicePrincipal.SingleOrDefault();
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
        .Request()
        .Filter($"resourceId eq {servicePrincipal.Id}")
        .GetAsync();

            var appRoleIds = userAppRoleAssignments.Select(a => a.AppRoleId);

            var appRoles = servicePrincipal.AppRoles
                .Where(a => appRoleIds.Contains(a.Id))
                .Select(a => a.Value);

            return appRoles.ToArray();
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    private static IEnumerable<Models.User> ToUserObjects(IGraphServiceUsersCollectionPage graphUsers) => 
        graphUsers.Select(graphUser => new Models.User()
        {
            UserId = graphUser.Id,
            DisplayName = graphUser.DisplayName
        });

}

