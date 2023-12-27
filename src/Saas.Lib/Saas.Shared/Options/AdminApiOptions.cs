
namespace Saas.Shared.Options;

public record AdminApiOptions
{
    public const string SectionName = "AdminApi";

    public string? ApplicationIdUri { get; init; }
    public string[]? Scopes { get; init; }
}
