CREATE TABLE [dbo].[ExternalShare]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[UniqueIdentifier] UNIQUEIDENTIFIER NOT NULL,
    [SiteCollectionUrl] NVARCHAR(256) NOT NULL,
    [LoginName] NVARCHAR(128) NOT NULL,
	[UserId] INT NOT NULL,
	[InvitedBy] NVARCHAR(128) NOT NULL,
    [OriginalSharedDate] DATETIME NOT NULL,
    [RefreshSharedDate] DATETIME NULL, 
    [LastProcessedDate] DATETIME NULL
)
