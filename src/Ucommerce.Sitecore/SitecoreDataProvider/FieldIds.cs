using Sitecore.Data;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	public static class FieldIds
	{
		public static class Common
		{
			public static ID UCommerceRootTemplateId { get { return ID.Parse("{AABC1CFA-9CDB-4AE5-8257-799D84A8EE23}"); } }
			public static ID UCommerceStoresTemplateId { get { return ID.Parse("{05B44E49-0D2E-4AED-9F37-B90B3E6144DC}"); } }
			public static ID UCommerceCatalogNodeItemId { get { return ID.Parse("{432FDDBE-37F4-4C49-8ABE-3DF838A2B2CC}"); } }
		}
		public static class Store
		{
			// Common section
			public static ID SectionCommonId { get { return ID.Parse("{6D39A20F-A659-48C2-9D63-7425F532D818}"); } }

			// Common fields
			public static ID NameFieldId { get { return ID.Parse("{EB53F364-7CEE-4E54-A9F5-2ABFC6EE0613}"); } }
			public static ID ProductCatalogGroupIdFieldId { get { return ID.Parse("{3BACEC54-2804-448B-84BF-152ECA8E2950}"); } }
			public static ID HostNameFieldId { get { return ID.Parse("{EE8D55CD-0AE9-4748-BBF1-E842990CB667}"); } }
			public static ID DescriptionFieldId { get { return ID.Parse("{7A3CAC47-BCC9-4534-AEB1-302358CFF5B4}"); } }
			public static ID CurrencyFieldId { get { return ID.Parse("{A5F26030-7A7A-4F57-ADB5-CC17D8517FF5}"); } }
			public static ID EmailProfileFieldId { get { return ID.Parse("{AFE9EADF-4ADC-4DB5-98B8-539556750604}"); } }
			public static ID OrderNumberSeriesFieldId { get { return ID.Parse("{75EC6678-0910-4003-BAC5-0D3A459817A7}"); } }
			public static ID ProductReviewRequiresApprovalFieldId { get { return ID.Parse("{180F6A38-B927-4CE6-8CBC-160EBEAF314D}"); } }
			public static ID CreateCustomersAsMembersFieldId { get { return ID.Parse("{A5CFF9F2-595B-45A8-862E-B3F0B8EE1CDE}"); } }
			public static ID MemberGroupFieldId { get { return ID.Parse("{7290E13D-1BDA-413C-BBAA-5F4497458797}"); } }
			public static ID MemberTypeFieldId { get { return ID.Parse("{5A38A207-6EE5-4351-A368-0DE37455819D}"); } }
		}

		public static class Category
		{
			public static ID ProductCategoryTemplateId { get { return ID.Parse("{ABD9F13F-DFAC-4ED9-8C23-E0A9C5BB19C9}"); } }

			// Common section
			public static ID SectionCommonId { get { return ID.Parse("{0AD9DB4C-7A72-4BFA-AACB-B83C0A17B964}"); } }
			// Common fields
			public static ID NameFieldId { get { return ID.Parse("{CB4745BB-E2E4-435A-860F-326B8B9EF6C8}"); } }
			public static ID CategoryIdFieldId { get { return ID.Parse("{706DD7B3-15F3-4D8E-8405-EAED3F2FB21A}"); } }
			public static ID ParentCatalogIdFieldId { get { return ID.Parse("{34CFFE7A-CC74-4816-8299-541511A4CA37}"); } }
			public static ID DisplayOnSiteFieldId { get { return ID.Parse("{09C49F1A-80F7-44E4-B9F3-22DFE1BC26A7}"); } }
			public static ID DisplayNameFieldId { get { return ID.Parse("{57F7921D-4841-4823-AE68-4DE3C1E8E869}"); } }
			public static ID DescriptionFieldId { get { return ID.Parse("{E483B50F-CE23-4C50-9099-D9EA3476043D}"); } }

			// Media section
			public static ID SectionMediaId { get { return ID.Parse("{30AD05EC-89C1-4332-9F54-10EC61EAD21D}"); } }
			// Media fields
			public static ID ImageFieldId { get { return ID.Parse("{11BA63F4-2B55-4F43-96A3-21B129E2130B}"); } }

			// Audit section
			public static ID SectionAuditInformation { get { return ID.Parse("{FCBB6102-6301-42B6-8E4A-897A3ECBFA6D}"); } }
			// Audit fields
			public static ID CreatedOnFieldId { get { return ID.Parse("{177A654C-6F74-43DD-B6C4-9557EC4173C8}"); } }
			public static ID ModifiedOnFieldId { get { return ID.Parse("{02EA3BC8-5796-4CBB-999B-92E5C7D6985E}"); } }
			public static ID CreatedByFieldId { get { return ID.Parse("{2AB0E29B-509A-480D-98B4-76234D9383BD}"); } }
			public static ID ModifiedByFieldId { get { return ID.Parse("{323F4547-3428-494F-B283-4925A0D9D834}"); } }
			public static ID SectionDynamicFields
			{
				get { return ID.Parse("{390DC85A-4F28-4E8D-8BBF-99696787547C}"); }
			}

			public static ID ProductsInCategory { get { return ID.Parse("{0B73DF7C-12A2-4F4F-AF00-48E30443F13B}"); } }
		}

		public static class Product
		{
			public static ID ProductBaseTemplateId { get { return ID.Parse("{86FA3A87-B0A3-4A4B-BBE1-E75398D267D5}"); } }

			public static ID ProductsRootFolderId { get { return ID.Parse("{A04FACE7-36CB-460B-8AEC-8B3BC7AF5EB8}"); } }

			// Base product sections
			public static ID SectionCommonId { get { return ID.Parse("{36231CE9-092C-4502-B41A-9FECBAC98B47}"); } }
			public static ID SectionPricingId { get { return ID.Parse("{8A70E815-EF0A-4072-AC49-1C828BD67949}"); } }
			public static ID SectionMediaId { get { return ID.Parse("{8DA7D963-082A-4C4C-B2CD-B4D11EA52DE4}"); } }
			public static ID SectionCategoriesId { get { return ID.Parse("{CDD5A4F3-65A9-4CFB-A6D7-E4CB4F18A37B}"); } }
			public static ID SectionRelatedProductsId { get { return ID.Parse("{BF9D21FD-4F8F-4D2D-9313-EFB21706989B}"); } }
			public static ID SectionReviewsId { get { return ID.Parse("{0A07BA41-CC11-49D5-977A-A5E09BE2484C}"); } }
			public static ID SectionAuditId { get { return ID.Parse("{907988FB-B450-4B91-8B42-FAF040F61F17}"); } }

			// Common fields
			public static ID SkuFieldId { get { return ID.Parse("{29A056FB-035E-4E86-9D28-56618DE361DF}"); } }
			public static ID ProductIdFieldId { get { return ID.Parse("{3AE8B54B-4D74-4900-9095-9F29F284D07E}"); } }
			public static ID InternalNameFieldId { get { return ID.Parse("{16626795-6959-4B11-BA78-BE41302C6388}"); } }
			public static ID DisplayOnSiteFieldId { get { return ID.Parse("{391DEDDF-E2E2-47D1-9784-656197DD342A}"); } }
			public static ID AllowOrderingFieldId { get { return ID.Parse("{C6E2B704-3C0C-45A0-B268-7B84657229B7}"); } }

			// Categories fields
			public static ID CategoriesTreeListId { get { return ID.Parse("{8D90D02A-3B7C-4ACA-AE9D-CF968068B444}"); } }

			// Media fields
			public static ID ThumbnailFieldId { get { return ID.Parse("{1485E0CA-E1F6-4756-AAEB-08A91C5F6DC9}"); } }
			public static ID PrimaryImageFieldId { get { return ID.Parse("{41E0C41F-DF65-42A9-A696-EEAD1190F1EA}"); } }

			// Multi-lingual fields
			public static ID DisplayNameFieldId { get { return ID.Parse("{981291A0-66DC-48FB-A83E-F549C783ADC6}"); } }
			public static ID ShortDescriptionFieldId { get { return ID.Parse("{58E94702-ED8D-4E74-8144-56F6137AF3BA}"); } }
			public static ID LongDescriptionFieldId { get { return ID.Parse("{0C598BB4-37D7-4DB9-AF12-2544CE944DB8}"); } }

			// Audit fields
			public static ID CreatedOnFieldId { get { return ID.Parse("{BE45A2A3-C823-4157-BC44-78E3F9E6084B}"); } }
			public static ID CreatedByFieldId { get { return ID.Parse("{BCD9B3C3-06D8-4F29-AD33-4D53BFD00F73}"); } }
			public static ID ModifiedOnFieldId { get { return ID.Parse("{3F06FE1B-48D6-4DD5-9858-8D769BF519AF}"); } }
			public static ID ModifiedByFieldId { get { return ID.Parse("{7FAFB6B6-3238-4A16-BBFA-58F9789B0682}"); } }

			// Product relations
			public static ID ProductRelations { get { return ID.Parse("{D7E29753-F341-4778-BC2F-2FD11843FB9D}"); } }
		}

		public static class Variant
		{
			// Base variant sections
			public static ID SectionCommonId { get { return ID.Parse("{82BE5812-D3E3-4C1C-8101-88436764287C}"); } }
			public static ID SectionPricingId { get { return ID.Parse("{DD8FBEEF-B6EC-4174-831E-66CDA4686710}"); } }

			// Common fields
			public static ID SkuFieldId { get { return ID.Parse("{0A3FBD5B-4936-46D2-9911-2EA7353DAA44}"); } }
			public static ID ProductIdFieldId { get { return ID.Parse("{81718C0F-A7AC-4837-B35C-E84BE0BDF130}"); } }
			public static ID InternalNameFieldId { get { return ID.Parse("{BDED4729-3A80-4EAB-869A-8EECF7EF50A8}"); } }

			// Multi-lingual fields
			public static ID DisplayNameFieldId { get { return ID.Parse("{3C5DE3AF-FE34-468A-AE16-31294CF27966}"); } }
			public static ID ShortDescriptionFieldId { get { return ID.Parse("{72DB741A-756B-4B8D-830E-CCBCE50CE524}"); } }
			public static ID LongDescriptionFieldId { get { return ID.Parse("{C07438FE-EBBB-4BDA-8111-821C29D9BDA6}"); } }
		}

		public static class Catalog
		{
			public static ID ProductCatalogTemplateId { get { return ID.Parse("{5EDC25C8-CC1A-4ECC-9E26-2D4FF0219F78}"); } }

			public static ID SectionCommonId { get { return ID.Parse("{E19F81B5-7C3E-4BC6-9C86-4A3EC9892E64}"); } }
			public static ID SectionAllowedPriceGroupsId { get { return ID.Parse("{9497A696-A188-45B6-A008-EE1D98BC58BD}"); } }

			public static ID NameFieldId { get { return ID.Parse("{5281C13D-B06D-469B-B8E4-5E01AC179AF5}"); } }
			public static ID CatalogIdFieldId { get { return ID.Parse("{6850B9BB-0E4F-4AC8-A5D2-4BC9AA9BE620}"); } }
			public static ID DisplayNameFieldId { get { return ID.Parse("{8381BB59-5106-4757-883F-FB658D9ACBC1}"); } }
			public static ID DefaultPriceGroupId { get { return ID.Parse("{3F24B57E-1FAE-43E3-A0BF-006C81BC917E}"); } }
			public static ID ShowPricesWithVatId { get { return ID.Parse("{5A5AA792-7A74-4507-9F7B-7A228AFD3056}"); } }
		}

		public static class Template
		{
			public static ID UCommerceTemplateFolderId { get { return ID.Parse("{A3585578-10F9-48F3-AFE4-B568AB275A6F}"); } }
		}

		public static class SystemContent
		{
			public static ID UCommerceSystemDataFolderId { get { return ID.Parse("{8E53881D-555E-4B05-BC40-ADF4D23167C6}"); } }

			public static ID CurrenciesFolderId { get { return ID.Parse("{FB6716FF-96DB-41F1-ACD1-741110F01F5A}"); } }
			public static ID EmailProfilesFolderId { get { return ID.Parse("{9CF6A75F-DFC8-4161-B296-95CB86C426A0}"); } }
			public static ID MemberGroupsFolderId { get { return ID.Parse("{4065414C-54B9-44AC-B294-88BA89B0D9D7}"); } }
			public static ID MemberTypeFolderId { get { return ID.Parse("{3B716FCE-6DF5-4462-8E27-05E2B510C11C}"); } }
			public static ID HostnamesFolderId { get { return ID.Parse("{D7AA7902-6B7C-428A-8E09-3017D05BE23A}"); } }
			public static ID OrderNumberSeriesFolderId { get { return ID.Parse("{5FB29901-FDA9-4571-919A-537F2873CFDB}"); } }
			public static ID PriceGroupsFolderId { get { return ID.Parse("{A5DE0160-736E-4903-8D29-490CBEB274DF}"); } }
			public static ID DataTypeEnums { get { return ID.Parse("{EDBA8658-2FC7-47D0-BF3E-D0B518E57DEF}"); } }

			public static ID DefaultHostnameId { get { return ID.Parse("{B7946EE1-1098-4560-9563-FD61476686F0}"); } }

			public static ID EmptyContextMenuNodeId { get { return ID.Parse("{D09D126F-A9A0-4744-BE3C-A97143D8CFEF}"); } }

			public static ID UcommerceProductsMultilistWithSearch { get { return ID.Parse("{F82ED8A3-7E45-438B-8EAF-93ED789B2FBB}"); } }

			public static ID UcommerceCategoriesTreeList { get { return ID.Parse("{EE9267F6-CF46-4D49-B04B-AD5F09094F22}"); } }
		}
	}
}
