using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saas.Catalog.Api.Models;
using Saas.Catalog.Api.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Catalog.Api.Tests.Services
{
    [TestClass]
    public class TenantServiceTests
    {
        private ITenantService _tenantService;
        
        public TenantServiceTests()
        {
            _tenantService = new TenantService(Context);
        }

        [TestInitialize]
        public void Setup()
        {
            _tenantService = new TenantService(Context);
        }

        [TestMethod]
        public async Task TenantService_GetItems_EmptyReturnsNone()
        {
            //Arrange
            
            //Act
            var results = await _tenantService.GetItemsAsync();

            //Assert
            Assert.IsFalse(results.Any());
        }

        [TestMethod]
        public async Task TenantService_GetItem_EmptyReturnsNone()
        {
            //Arrange
            var guid = Guid.NewGuid();

            //Act
            var results = await _tenantService.GetItemAsync(guid);

            //Assert
            Assert.IsNull(results);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]  
        public async Task TenantService_AddItemWithoutRequired_Throws()
        {
            //Arrange
            var tenant = new Tenant();

            //Act
            await _tenantService.AddItemAsync(tenant);

            //Assert
            // Expected Exception Microsoft.EntityFrameworkCore.DbUpdateException
        }

        [TestMethod]
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
            Assert.AreNotEqual(beforeCount, afterAddCount);
            Assert.IsTrue(afterAddCount == 1);
        }

        [TestMethod]
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
            Assert.IsTrue((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.AreEqual(result, tenant1);
        }

        [TestMethod]
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
            Assert.IsTrue((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
            Assert.IsNull(result);
        }


        [TestMethod]
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
            Assert.IsTrue((await _tenantService.GetItemsAsync()).Count<Tenant>() == 2);
        }


        [TestMethod]
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
            Assert.IsTrue((await _tenantService.GetItemsAsync()).Count<Tenant>() == 1);
        }

        [TestMethod]
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
            Assert.AreEqual(tenant1, tenantFromDB);

            //Act
            var updatedName = "Updated Name";
            tenant1.Name = updatedName;
            await _tenantService.UpdateItemAsync(tenant1);
            var updatedTenantFromDB = await _tenantService.GetItemAsync(tenant1.Id);

            //Assert
            Assert.IsTrue(updatedTenantFromDB.Name == updatedName);
        }

        [TestMethod]
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
            Assert.AreEqual("Test tenant 1", updatedTenantFromDB.Name);
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
