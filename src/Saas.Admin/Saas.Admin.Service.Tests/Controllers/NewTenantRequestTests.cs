namespace Saas.Admin.Service.Tests
{
    using System;
    using Xunit;

    public class NewTenantRequestTests
    {
        [Theory, AutoDataNSubstitute]
        public void All_Values_Are_Copied_To_Tenant(NewTenantRequest tenantRequest)
        {
            Tenant tenant = tenantRequest.ToTenant();

            AssertAdditions.AllPropertiesAreEqual(tenant, tenantRequest, nameof(tenant.ConcurrencyToken), nameof(tenant.CreatedTime), nameof(tenant.Id));
        }
    }
}