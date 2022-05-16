namespace Saas.Admin.Service;

public static class AppConstants
{
    public static class Policies
    {
        public const string Authenticated = "Authenticated";
        public const string CreateTenant = "Create_Tenant";
        public const string TenantGlobalRead = "Tenant_Global_Read";
        public const string TenantRead = "Tenant_Read";
        public const string TenantWrite = "Tenant_Write";
        public const string TenantDelete = "TenantDelete";
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
        public const string GlobalWrite = "tenant.global.write";
        public const string Read = "tenant.read";
        public const string Write = "tenant.write";
        public const string GlobalDelete = "tenant.global.delete";
        public const string Delete = "tenant.delete";
    }
}
