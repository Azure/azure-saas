namespace Saas.Shared.Options;

public record SqlOptions
{
    public const string SectionName = "Sql";

    public string? SQLAdministratorLoginName { get; init; }
    public string? TenantSQLConnectionString { get; init; }
    public string? PermissionsSQLConnectionString { get; init; }

    //test sql connection string
    public string? IbizzSaasConnectionString { get; init; }

}
