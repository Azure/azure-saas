
namespace Saas.Identity.Authorization.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class SaasRequirementAttribute(string name) : System.Attribute
{
    public string PermissionEntityName { get; } = name;
}
