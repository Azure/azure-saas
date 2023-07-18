using Saas.Shared.Interface;

namespace Saas.Shared.Options;

public class SaasApiScopeOptions<TProvider> where TProvider : ISaasApi
{
    public const string SectionName = "Scopes";

    public string[]? Scopes { get; set; }
}
