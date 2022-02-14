namespace Saas.Catalog.Api.Tests.Models
{
    using Saas.Catalog.Api.Models;
    using Saas.Domain.Models;

    using System;
    using System.Linq;
    using System.Reflection;

    using TestUtilities;

    using Xunit;

    public class CatalogTenantTests
    {

        [Theory, AutoDataNSubstitute]
        public void WillCopyAllValuesToTenant(CatalogTenant catalogTenant)
        {
            Tenant tenant = CatalogTenant.ToTenant(catalogTenant);
            AssertAdditions.AllPropertiesAreEqual(catalogTenant, tenant);
        }

        [Theory, AutoDataNSubstitute]
        public void WillCopyAllValuesToCatalogTenant(Tenant tenant)
        {
            CatalogTenant catalogTenant = CatalogTenant.FromTenant(tenant);
            AssertAdditions.AllPropertiesAreEqual(tenant, catalogTenant);
        }
    }
}