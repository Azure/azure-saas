namespace Saas.Identity.Authorization.Model;
public sealed record SaasPolicy
{
    internal string GroupName { get; init; }
    internal string Value { get; init; }
    internal string? RoutingKeyName { get; init; }

    public SaasPolicy(string policyStr)
    {
        var policyElements = policyStr.Split('.');

        if (policyElements.Length < 2)
        {
            throw new ArgumentException("Incomplate policy string. Must contain at least two elements or more.");
        }

        GroupName = policyElements[0];
        Value = policyElements[1];
        
        if (policyElements.Length > 2 && !string.IsNullOrEmpty(policyElements[2]))
        {
            RoutingKeyName = policyElements[2];
        }
    }
}
