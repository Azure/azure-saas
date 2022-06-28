namespace Saas.Billing.Service;

public static class AppConstants
{
    public static class Policies
    {
        public const string Authenticated = "Authenticated";
        public const string GlobalAdmin = "Global_Admin";
        public const string TenantGlobalRead = "Tenant_Global_Read";
        public const string TenantRead = "Tenant_Read";
        public const string CreateTenant = "Create_Tenant";
    }

    public static class Roles
    {
        public const string GlobalAdmin = "GlobalAdmin";
        public const string TenantUser = "TenantUser";
        public const string TenantAdmin = "TenantAdmin";
        public const string Self = "Self";
    }

    public static class Scopes
    {
        public const string GlobalRead = "tenant.global.read";
        public const string Read = "tenant.read";
    }
}
