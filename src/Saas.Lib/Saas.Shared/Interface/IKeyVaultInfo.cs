﻿namespace Saas.Interface;

public interface IKeyVaultInfo
{
    string? KeyVaultUrl { get; }
    string? KeyVaultCertificateName { get; }
}