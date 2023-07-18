namespace Saas.Identity.Authorization.Option;

public record SaasAuthorizationOptions
{
    public const string SectionName = "SaasAuthorization";

    public Guid? Global { get; init; }
}
