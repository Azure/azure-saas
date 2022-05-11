using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;
using Saas.Permissions.Service.Models.AppSettings;
namespace Saas.Permissions.Service.Services;


public class GraphAPIService : IGraphAPIService
{
    private readonly GraphServiceClient _graphServiceClient;

    public GraphAPIService(IOptions<AzureADB2COptions> options)
    {

        ClientSecretCredential clientSecretCredential = new ClientSecretCredential(
           options.Value.TenantId, options.Value.ClientId, options.Value.ClientSecret);


        this._graphServiceClient = new GraphServiceClient(clientSecretCredential);


    }
    public async Task<string[]> GetAppRolesAsync(ClaimsRequest request)
    {
        ServicePrincipal? servicePrincipal = await GetServicePrincipalAsync(request.ClientId);

        if(servicePrincipal == null)
        {
            throw new ArgumentException($"App role not found for \"{request.ClientId}\".");
        }

        return await GetAppRoleAssignmentsAsync(servicePrincipal, request.ObjectId.ToString());
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
}

