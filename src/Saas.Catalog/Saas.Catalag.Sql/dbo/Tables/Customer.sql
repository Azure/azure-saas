CREATE TABLE [dbo].[Customer]
(
    Id uniqueidentifier NOT NULL,  
    TenantId uniqueidentifier NOT NULL,  
    CustomerName nvarchar(50) NOT NULL,  
    IsActive bit NOT NULL, 
    CONSTRAINT [PK_Customer] PRIMARY KEY ([Id]),
    CONSTRAINT FK_Customer_Tenant
    FOREIGN KEY (TenantId)
    REFERENCES dbo.Tenant(Id)
)
