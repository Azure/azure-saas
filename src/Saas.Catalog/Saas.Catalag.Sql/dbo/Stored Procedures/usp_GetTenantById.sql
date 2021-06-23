CREATE PROCEDURE [dbo].[usp_GetTenantById]
	@tenantId nvarchar (37)

AS

SELECT [Tenant].[Id]
      ,[Tenant].[Name]
	  ,[DatabaseServer]
	  ,[DatabaseName]
	  ,[WebAppName]
	  ,[StorageContainerName]
	  ,[Url]
      ,[CreatedOn]
	  ,[ProductId]
	  ,[Product].[Name] AS ProductName
	  ,[Product].[Tier] AS ProductTier
	  ,[Tenant].[UserName]
	  ,[AspNetUsers].[Email]
	  ,[AspNetUsers].[Id]
  FROM [dbo].[Tenant]
  INNER JOIN Product ON Tenant.ProductId = Product.Id
  INNER JOIN AspNetUsers ON Tenant.UserId = AspNetUsers.Id
  WHERE [Tenant].[Id] = @tenantId

RETURN 0
