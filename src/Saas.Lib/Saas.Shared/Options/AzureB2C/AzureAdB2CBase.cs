﻿using Saas.Interface;

namespace Saas.Shared.Options;

public record AzureAdB2CBase
{
    public string? ClientId { get; init; }
    public string? Audience { get; init; }
    public string? Domain { get; init; }
    public string? Instance { get; init; }
    public string? SignedOutCallbackPath { get; init; }
    public string? SignUpSignInPolicyId { get; init; }
    public string? TenantId { get; init; }
    public string? LoginEndpoint { get; init; }
    public string? BaseUrl { get; init; }
    public string? Certificate { get; init; }
    public string? ClientSecret { get; init; }

    public KeyVaultCertificate[]? KeyVaultCertificateReferences { get; init; }
}
    

public record KeyVaultCertificate : IKeyVaultInfo
{
    public string? SourceType { get; init; }
    public string? KeyVaultUrl { get; init; }
    public string? KeyVaultCertificateName { get; init; }
}