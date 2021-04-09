CREATE TABLE [dbo].[Tenant]
(
    Id uniqueidentifier NOT NULL,
    ApiKey uniqueidentifier NOT NULL,
    TenantName nvarchar(200) NOT NULL,
    IsActive bit NOT NULL, 
    CONSTRAINT [PK_Tenant] PRIMARY KEY ([Id])
)
