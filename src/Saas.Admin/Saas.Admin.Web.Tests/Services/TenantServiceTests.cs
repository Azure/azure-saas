using Microsoft.EntityFrameworkCore;

using Saas.Admin.Web.Models;
using Saas.Admin.Web.Services;
using Saas.Domain.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

using TestUtilities;

using Xunit;

namespace Saas.Admin.Web.Tests.Services
{
    public class TenantServiceTests
    {
        private ITenantService _tenantService;

        public TenantServiceTests()
        {
            _tenantService = new TenantService(Context);
        }

        [Fact]
        public async Task TenantService_GetItems_EmptyReturnsNone()
        {
            //Arrange

            //Act
            var results = await _tenantService.GetItemsAsync();

            //Assert
            Assert.False(results.Any());
        }

        [Fact]
        public async Task TenantService_GetItem_EmptyReturnsNone()
        {
            //Arrange
            var guid = Guid.NewGuid();

            //Act
            var results = await _tenantService.GetItemAsync(guid);

            //Assert
            Assert.Null(results);
        }


        [Theory, AutoDataNSubstitute]
        public async Task TenantService_AddItemWithRequired_Adds(Tenant tenant)
        {
            //Act
            var beforeCount = (await _tenantService.GetItemsAsync()).Count<Tenant>();

            await _tenantService.AddItemAsync(tenant);

            //Assert
            int afterAddCount = (await _tenantService.GetItemsAsync()).Count<Tenant>();
            Assert.NotEqual(beforeCount, afterAddCount);
            Assert.True(afterAddCount == 1);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItemValid_ReturnsTenant(Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);
            await _tenantService.AddItemAsync(tenant2);

            //Act
            var result = await _tenantService.GetItemAsync(tenant1.Id);
            Assert.NotNull(result);
            Assert.Equal(result.Id, tenant1.Id);
            Assert.Equal(result.Name, tenant1.Name);

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_GetItemInvalid_Null(Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);
            await _tenantService.AddItemAsync(tenant2);


            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.Null(await _tenantService.GetItemAsync(Guid.NewGuid()));
            Assert.NotNull(await _tenantService.GetItemAsync(tenant1.Id));
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_DeleteItemInvalid_DeletesNothing(Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);
            await _tenantService.AddItemAsync(tenant2);

            //Act
            await _tenantService.DeleteItemAsync(Guid.NewGuid());

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
        }


        [Theory, AutoDataNSubstitute]
        public async Task TenantService_DeleteItem_DeletesTenant(Tenant tenant1, Tenant tenant2)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);
            await _tenantService.AddItemAsync(tenant2);

            //Act
            await _tenantService.DeleteItemAsync(tenant1.Id);

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 1);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_UpdateItem_UpdatesTenant(Tenant tenant1)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);
            var tenantFromDB = await _tenantService.GetItemAsync(tenant1.Id);
            Assert.Equal(tenant1, tenantFromDB);

            //Act
            var updatedName = "Updated Name";
            tenant1.Name = updatedName;
            await _tenantService.UpdateItemAsync(tenant1);
            var updatedTenantFromDB = await _tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.True(updatedTenantFromDB.Name == updatedName);
        }

        [Theory, AutoDataNSubstitute]
        public async Task TenantService_UpdateInvalidItem_NoUpdate(Tenant tenant1, Tenant updatedItem)
        {
            //Arrange
            await _tenantService.AddItemAsync(tenant1);

            //Act
            updatedItem.UserId = tenant1.UserId;
            updatedItem.IsActive = tenant1.IsActive;

            await _tenantService.UpdateItemAsync(updatedItem);
            var updatedTenantFromDB = await _tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.NotEqual(tenant1.Name, updatedItem.Name);
            Assert.Equal(tenant1.Name, updatedTenantFromDB.Name);
        }


        public CatalogDbContext Context => InMemoryContext();
        private CatalogDbContext InMemoryContext()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            var context = new CatalogDbContext(options);

            return context;
        }

    }
}
