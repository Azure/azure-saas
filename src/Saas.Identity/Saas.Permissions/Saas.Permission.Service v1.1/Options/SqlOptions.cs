namespace Saas.Permissions.Service.Options;

public record SqlOptions
{
    public const string SectionName = "Sql";

    public string? SQLAdministratorLoginName { get; init; }
    public string? SQLConnectionString { get; init; }

}
