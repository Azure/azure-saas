namespace Saas.Shared.Options;

public record SqlOptions
{
    public const string SectionName = "Sql";
    public const string DefaultDb = "IbizzSaasConnectionString";

    public string? SQLAdministratorLoginName { get; init; }
    public string? TenantSQLConnectionString { get; init; }
    public string? PermissionsSQLConnectionString { get; init; }

    //test sql connection string
    public string? IbizzSaasConnectionString { get; init; }

    //Azure regions
    public string? NorthCentalUS { get; init; }

}
