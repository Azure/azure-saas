CREATE TABLE [dbo].[Product]
(
    [Id] INT NOT NULL,
	[Name] NVARCHAR(250) NOT NULL,  
	[Tier] NVARCHAR(50) NULL, 
	[Description] NTEXT NOT NULL, 
	[ListPrice] MONEY NOT NULL, 
    [BlobName] NVARCHAR(250) NOT NULL, 
    [Created] DATETIME NOT NULL DEFAULT (getdate()), 
	[DownloadCount] INT NOT NULL DEFAULT 0,  

    
    [Icon] NVARCHAR(250) NULL, 
    PRIMARY KEY ([Id])
)
