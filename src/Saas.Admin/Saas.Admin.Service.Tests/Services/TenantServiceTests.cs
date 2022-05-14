namespace Saas.Admin.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoFixture.Xunit2;

    using Microsoft.EntityFrameworkCore;

    using Saas.Admin.Service.Data;
    using Saas.Admin.Service.Exceptions;
    using Saas.Admin.Service.Services;

    using TestUtilities;

    using Xunit;

    public class TenantServiceTests
    {

        [Theory, AutoDataNSubstitute]
        public async Task Will_Not_Return_Null_When_No_Tenants(TenantService tenantService)
        {
            IEnumerable<TenantDTO>? result = await tenantService.GetAllTenantsAsync();
            Assert.NotNull(result);
            Assert.Empty(result.ToList());
        }

        [Theory, AutoDataNSubstitute]
        public async Task Will_throw_if_tenenent_Not_Found([Frozen] TenantsContext tenantsContext, TenantService tenantService, Guid tenantId)
        {
            Assert.Null(await tenantsContext.Tenants.FindAsync(tenantId));

            await Assert.ThrowsAsync<ItemNotFoundExcepton>(() => tenantService.GetTenantAsync(tenantId));
        }

        [Theory, AutoDataNSubstitute]
        public async Task Will_throw_if_tenenent_Not_Found2([Frozen] TenantsContext tenantsContext, TenantService tenantService, Tenant[] tenants)
        {
            await tenantsContext.Tenants.AddRangeAsync(tenants);

            Guid id = tenants[^1].Id;
            TenantDTO? tenant = await tenantService.GetTenantAsync(id);

            AssertAdditions.AllPropertiesAreEqual(tenant, tenants[^1], nameof(tenant.CreatedTime), nameof(tenant.Version));
            await Assert.ThrowsAsync<ItemNotFoundExcepton>(() => tenantService.GetTenantAsync(Guid.NewGuid()));
        }

        [Theory, AutoDataNSubstitute]
        public async Task can_find_tenent([Frozen] TenantsContext tenantsContext, TenantService tenantService, Tenant[] tenants)
        {
            await tenantsContext.Tenants.AddRangeAsync(tenants);

            Guid id = tenants[^1].Id;
            TenantDTO? tenant = await tenantService.GetTenantAsync(id);

            AssertAdditions.AllPropertiesAreEqual(tenant, tenants[^1], nameof(tenant.CreatedTime), nameof(tenant.Version));
        }

        [Theory, AutoDataNSubstitute]
        public async Task Can_update_tenant([Frozen] TenantsContext tenantsContext, TenantService tenantService, Tenant[] originalTenants, TenantDTO updatedDto)
        {
            await tenantsContext.Tenants.AddRangeAsync(originalTenants);
            await tenantsContext.SaveChangesAsync();

            Tenant tenant = await tenantsContext.Tenants.FirstAsync();
            DateTime originalCreated = tenant.CreatedTime!.Value;

            updatedDto.Id = tenant.Id;
            updatedDto.Version = null;

            TenantDTO? updatedTenant = await tenantService.UpdateTenantAsync(updatedDto);

            AssertAdditions.AllPropertiesAreEqual(updatedDto, updatedTenant, nameof(updatedTenant.Version), nameof(updatedTenant.CreatedTime));
            Assert.Equal(originalCreated, updatedTenant.CreatedTime);
        }

        [Theory, AutoDataNSubstitute]
        public async Task Can_delete_tenant([Frozen] TenantsContext tenantsContext, TenantService tenantService, Tenant[] originalTenants)
        {
            await tenantsContext.Tenants.AddRangeAsync(originalTenants);
            await tenantsContext.SaveChangesAsync();

            Guid idToDelete = originalTenants[0].Id;

            Assert.NotNull(await tenantService.GetTenantAsync(idToDelete));

            await tenantService.DeleteTenantAsync(idToDelete);

            await Assert.ThrowsAsync<ItemNotFoundExcepton>(() => tenantService.GetTenantAsync(idToDelete));
        }


        [Theory, AutoDataNSubstitute]
        public async Task Check_For_Existing_Route([Frozen] TenantsContext tenantsContext, TenantService tenantService, Tenant[] originalTenants, string notInDBRoute)
        {
            await tenantsContext.Tenants.AddRangeAsync(originalTenants);
            await tenantsContext.SaveChangesAsync();

            bool exists = await tenantService.CheckPathExists(originalTenants[0].Route);
            Assert.True(exists);

            bool notExists = await tenantService.CheckPathExists(notInDBRoute);
            Assert.False(notExists);
        }
    }
}