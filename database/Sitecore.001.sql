IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'uCommerce_SystemVersionSitecore')
BEGIN
	CREATE TABLE [dbo].[uCommerce_SystemVersionSitecore](
		[SystemVersionSitecoreId] [int] IDENTITY(1,1) NOT NULL,
		[SchemaVersion] [int] NOT NULL
	)

	INSERT INTO [dbo].[uCommerce_SystemVersionSitecore] (SchemaVersion) VALUES (0)
END