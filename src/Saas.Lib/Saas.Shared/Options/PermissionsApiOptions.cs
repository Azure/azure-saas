
namespace Saas.Shared.Options;

public class PermissionsApiOptions
{
    public const string SectionName = "PermissionsApi";

    public string? ApiKey { get; init; }

    public string? SQLConnectionString { get; init; }
}