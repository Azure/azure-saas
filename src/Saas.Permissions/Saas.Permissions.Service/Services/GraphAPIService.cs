using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Saas.Permissions.Service.Interfaces;
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
    public async Task<ICollection<string>> GetAppRolesAsync(string userId)
    {
        var appRoleAssignments = await this._graphServiceClient.Users[userId].AppRoleAssignments
                .Request()
                .GetAsync();
        var appRoleIds = appRoleAssignments
                .Select(a => a.AppRoleId.ToString())
                .ToArray()
                .Cast<string>()
                .ToArray();

        return appRoleIds;
                

    }
}

