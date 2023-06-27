using Microsoft.Azure.Cosmos;

namespace Saas.SignupAdministration.Web.Services;

public class UserBookingService : IUserBookingService
{
    private readonly CosmosDbOptions cosmosDb;
    public UserBookingService(IOptions<CosmosDbOptions> options)
    {
        cosmosDb = options.Value;  
    }


    // The Cosmos client instance
    private static CosmosClient cosmosClient;

    // The database we will create
    private static Database database;

    // The container we will create.
    private static Container container;

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
            cosmosClient = new CosmosClient(cosmosDb.EndpointURI, cosmosDb.PrimaryKey);
        }
        if (container == null)
        {
            container = cosmosClient.GetContainer(cosmosDb.IBusinessDatabaseId, cosmosDb.IBusinessContainerId);
        }
    }

}
