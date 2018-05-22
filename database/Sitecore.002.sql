IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'uCommerce_SharedField')
BEGIN
	CREATE TABLE [dbo].[uCommerce_SharedField] (
		[SharedFieldId] [int] IDENTITY(1,1) NOT NULL,
		[ItemId] [uniqueidentifier] NOT NULL,
		[FieldId] [uniqueidentifier] NOT NULL,
		[FieldValue] [varchar](max) NULL
	)
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'uCommerce_UnversionedField')
BEGIN
	CREATE TABLE [dbo].[uCommerce_UnversionedField](
		[UnversionedFieldId] [int] IDENTITY(1,1) NOT NULL,
		[ItemId] [uniqueidentifier] NOT NULL,
		[FieldId] [uniqueidentifier] NOT NULL,
		[Language] [varchar](max) NULL,
		[FieldValue] [varchar](max) NULL
	)
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'uCommerce_VersionedField')
BEGIN
	CREATE TABLE [dbo].[uCommerce_VersionedField](
		[VersionedFieldId] [int] IDENTITY(1,1) NOT NULL,
		[ItemId] [uniqueidentifier] NOT NULL,
		[FieldId] [uniqueidentifier] NOT NULL,
		[Language] [varchar](max) NULL,
		[Version] [int] NULL,
		[FieldValue] [varchar](max) NULL
	)
END
