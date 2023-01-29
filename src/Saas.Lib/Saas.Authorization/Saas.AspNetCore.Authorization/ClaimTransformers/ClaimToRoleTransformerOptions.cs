
namespace Saas.AspNetCore.Authorization.ClaimTransformers;

public record ClaimToRoleTransformerOptions
{
    public const string SectionName = "ClaimToRoleTransformer";

    public string? AuthenticationType { get; init; }
    public string? RoleClaimType { get; init; }
    public string? SourceClaimType { get; init; }
}
