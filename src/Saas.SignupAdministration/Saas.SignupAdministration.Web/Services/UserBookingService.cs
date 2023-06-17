using Microsoft.Azure.Cosmos;

namespace Saas.SignupAdministration.Web.Services;

public class UserBookingService : IUserBookingService
{
    private readonly IConfiguration _configuration;
    public UserBookingService(IConfiguration configuration)
    {
        _configuration = configuration;
        EndpointUri = _configuration["cosmosEndpoinUri"];
        PrimaryKey = _configuration["cosmosPrimaryKey"];
        DatabaseId = _configuration["IBusinessDatabaseId"];
        ContainerId = _configuration["IBusinessContainerId"];
    }


    // The Cosmos client instance
    private static CosmosClient cosmosClient;

    // The database we will create
    private static Database database;

    // The container we will create.
    private static Container container;

    private string EndpointUri;
    private string PrimaryKey;
    private string DatabaseId;
    private string ContainerId;

    public async Task<Booking> CreateBookingAsync(Booking booking, string partitionKey)
    {
        InitializeDatabaseInstance();

        try
        {
            // Read the item to see if it exists.  
            ItemResponse<Booking> bookingResponse = await container.CreateItemAsync<Booking>(booking, new PartitionKey(partitionKey));

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception();
        }

        return booking;
    }
    private void InitializeDatabaseInstance()
    {
        if (cosmosClient == null)
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        }
        if (container == null)
        {
            container = cosmosClient.GetContainer(DatabaseId, ContainerId);
        }
    }

}
