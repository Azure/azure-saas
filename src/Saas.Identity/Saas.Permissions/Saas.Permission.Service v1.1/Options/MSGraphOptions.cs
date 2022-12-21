namespace Saas.Permissions.Service.Options;

public record MSGraphOptions
{
    public const string SectionName = "MSGraph";

    public string? BaseUrl { get; init; }
    public string? Scopes { get; init; }
}
