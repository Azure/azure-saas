namespace Saas.SignupAdministration.Web.Models;

public record CosmosDbOptions
{
    public const string EndpointURISN = "cosmosEndpoinUri";
    public const string PrimaryKeySN = "cosmosPrimaryKey";
    public const string IBusinessContainerIdSN = "IBusinessContainerId";
    public const string IBusinessDatabaseIdSN = "IBusinessDatabaseId";
    public string? EndpointURI { get; set; }

    public string? PrimaryKey { get; set; }

    public string? IBusinessContainerId { get; set; }

    public string? IBusinessDatabaseId { get; set; }

    
}
