using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace Saas.SignupAdministration.Web;

// This is to use key name prefixes to only load in the secrets that pertain to this microservice
// https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix
public class CustomPrefixKeyVaultSecretManager : KeyVaultSecretManager
{
        private readonly string _prefix;

        public CustomPrefixKeyVaultSecretManager(string prefix)
            => _prefix = $"{prefix}-";

        public override bool Load(SecretProperties properties)
            => properties.Name.StartsWith(_prefix);

        public override string GetKey(KeyVaultSecret secret)
            => secret.Name[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter); 
}
