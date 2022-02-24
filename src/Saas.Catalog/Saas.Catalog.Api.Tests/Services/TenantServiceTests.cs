using Saas.Catalog.Api.Services;
using Saas.Domain.Exceptions;
using Saas.Domain.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

using TestUtilities;

using Xunit;

namespace Saas.Catalog.Api.Tests.Services
{
    public class TenantServiceTests
    {
        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItems_EmptyReturnsNone(TenantService tenantService)
        {
            //Arrange

            //Act
            var results = await tenantService.GetItemsAsync();

            //Assert
            Assert.False(results.Any());
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItem_EmptyReturnsNone(TenantService tenantService)
        {
            //Arrange
            var guid = Guid.NewGuid();

            //Act
            var results = await tenantService.GetItemAsync(guid);

            //Assert
            Assert.Null(results);
        }


        [Theory, AutoDataNSubstitute]
        public async Task TenantService_AddItemWithRequired_Adds(TenantService tenantService, Tenant tenant)
        {

            //Act
            var beforeCount = (await tenantService.GetItemsAsync()).Count<Tenant>();

            await tenantService.AddItemAsync(tenant);

            //Assert
            int afterAddCount = (await tenantService.GetItemsAsync()).Count<Tenant>();
            Assert.NotEqual(beforeCount, afterAddCount);

            Assert.True(afterAddCount == beforeCount + 1);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItemInvalid_ReturnsTenant(TenantService tenantService, Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);

            await tenantService.AddItemAsync(tenant2);

            //Act
            var result = await tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.True((await tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            AssertAdditions.AllPropertiesAreEqual(result, tenant1);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItemInvalid_Null(TenantService tenantService, Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);
            await tenantService.AddItemAsync(tenant2);

            //Act
            var result = await tenantService.GetItemAsync(Guid.NewGuid());

            //Assert
            Assert.True((await tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.Null(result);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_DeleteItemInvalid_TenantNotFoundException(TenantService tenantService, Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);
            await tenantService.AddItemAsync(tenant2);

            //Act
            var ex = await Assert.ThrowsAsync<TenantNotFoundException>(async () => await tenantService.DeleteItemAsync(Guid.NewGuid()));
        }


        [Theory, AutoDataNSubstitute]
        public async Task TenantService_DeleteItem_DeletesTenant(TenantService tenantService, Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);
            await tenantService.AddItemAsync(tenant2);

            //Act
            await tenantService.DeleteItemAsync(tenant1.Id);

            //Assert
            Assert.True((await tenantService.GetItemsAsync()).Count<Tenant>() == 1);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_UpdateItem_UpdatesTenant(TenantService tenantService, Tenant tenant1)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);
            var tenantFromDB = await tenantService.GetItemAsync(tenant1.Id);
            AssertAdditions.AllPropertiesAreEqual(tenant1, tenantFromDB);

            //Act
            var updatedName = "Updated Name";
            tenant1.Name = updatedName;
            await tenantService.UpdateItemAsync(tenant1);
            var updatedTenantFromDB = await tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.True(updatedTenantFromDB?.Name == updatedName);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_UpdateInvalidItem_TenantNotFoundException(TenantService tenantService, Tenant tenant1, Tenant updatedItem)
        {
            //Arrange
            await tenantService.AddItemAsync(tenant1);

            //Act
            updatedItem.IsActive = tenant1.IsActive;
            updatedItem.UserId = tenant1.UserId;

            var ex = await Assert.ThrowsAsync<TenantNotFoundException>(async () => await tenantService.UpdateItemAsync(updatedItem));
        }
    }
}
