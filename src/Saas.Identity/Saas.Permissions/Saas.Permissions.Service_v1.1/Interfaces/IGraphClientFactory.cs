using Microsoft.Graph;

namespace Saas.Permissions.Service.Interfaces;

public interface IGraphApiClientFactory
{
    GraphServiceClient Create();
}
