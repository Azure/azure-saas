using ClientAssertionWithKeyVault.Interface;

namespace Saas.Permissions.Service.Options;

public record PermissionApiOptions
{
    public const string SectionName = "PermissionApi";

    public string? Audience { get; init; }
    public string? ApiKey { get; init; }
    public string? ClientId { get; init; }
    public Certificate[]? Certificates { get; init; }
    public string? TenantId { get; init; }
    public string? Domain { get; init; }

}

public record Certificate : IKeyInfo
{
    public string? SourceType { get; init; }
    public string? KeyVaultUrl { get; init; }
    public string? KeyVaultCertificateName { get; init; }
}
