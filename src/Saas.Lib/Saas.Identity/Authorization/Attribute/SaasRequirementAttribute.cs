
namespace Saas.Identity.Authorization.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class SaasRequirementAttribute : System.Attribute
{
    public string PermissionEntityName { get; }

    public SaasRequirementAttribute(string name)
    {
        PermissionEntityName = name;
    }
}
