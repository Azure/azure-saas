using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace Saas.Admin.Service.Utilities;

// This is to use key name prefixes to only load in the secrets that pertain to this microservice
// https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix
public class CustomPrefixKeyVaultSecretManager : KeyVaultSecretManager
{
    private readonly string _prefix;

    public CustomPrefixKeyVaultSecretManager(string prefix)
    {
        _prefix = $"{prefix}-";
    }

    public override bool Load(SecretProperties properties)
    {
        return properties.Name.StartsWith(_prefix);
    }

    public override string GetKey(KeyVaultSecret secret)
    {
        return secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter);
    }
}
