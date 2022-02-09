using Microsoft.EntityFrameworkCore;
using Saas.Catalog.Api.Models;
using Saas.Catalog.Api.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Saas.Catalog.Api.Tests.Services
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

        [Fact]
        public async Task TenantService_AddItemWithoutRequired_Throws()
        {
            //Arrange
            var tenant = new Tenant();

            //Act
            var ex = await Assert.ThrowsAsync<DbUpdateException>(async () => await _tenantService.AddItemAsync(tenant));

            //Assert
            // Expected Exception Microsoft.EntityFrameworkCore.DbUpdateException
        }

        [Fact]
        public async Task TenantService_AddItemWithRequired_Adds()
        {
            //Arrange
            var tenant = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };

            //Act
            var beforeCount = (await _tenantService.GetItemsAsync()).Count<Tenant>();

            await _tenantService.AddItemAsync(tenant);

            //Assert
            int afterAddCount = (await _tenantService.GetItemsAsync()).Count<Tenant>();
            Assert.NotEqual(beforeCount, afterAddCount);
            Assert.True(afterAddCount == 1);
        }

        [Fact]
        public async Task TenantService_GetItemInvalid_ReturnsTenant()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant1);
            var tenant2 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 2",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant2);

            //Act
            var result = await _tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.Equal(result, tenant1);
        }

        [Fact]
        public async Task TenantService_GetItemInvalid_Null()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant1);
            var tenant2 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 2",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant2);

            //Act
            var result = await _tenantService.GetItemAsync(Guid.NewGuid());

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.Null(result);
        }

        [Fact]
        public async Task TenantService_DeleteItemInvalid_DeletesNothing()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant1);
            var tenant2 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 2",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant2);

            //Act
            await _tenantService.DeleteItemAsync(Guid.NewGuid());

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
        }


        [Fact]
        public async Task TenantService_DeleteItem_DeletesTenant()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant1);
            var tenant2 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 2",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant2);

            //Act
            await _tenantService.DeleteItemAsync(tenant1.Id);

            //Assert
            Assert.True((await _tenantService.GetItemsAsync()).Count<Tenant>() == 1);
        }

        [Fact]
        public async Task TenantService_UpdateItem_UpdatesTenant()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
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

        [Fact]
        public async Task TenantService_UpdateInvalidItem_NoUpdate()
        {
            //Arrange
            var tenant1 = new Tenant()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Name = "Test tenant 1",
                UserId = Guid.NewGuid().ToString()
            };
            await _tenantService.AddItemAsync(tenant1);

            //Act
            var updatedItem = new Tenant()
            {
                Id = Guid.NewGuid(),
                IsActive = tenant1.IsActive,
                Name = "Updated Name",
                UserId = tenant1.UserId
            };
            await _tenantService.UpdateItemAsync(updatedItem);
            var updatedTenantFromDB = await _tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.Equal("Test tenant 1", updatedTenantFromDB.Name);
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
