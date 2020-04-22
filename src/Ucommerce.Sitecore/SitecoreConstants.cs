using System;

namespace Ucommerce.Sitecore
{
	/// <summary>
	/// This class holds software constants used in the Sitecore project.
	/// </summary>
    internal static class SitecoreConstants
    {
		public const string SitecoreIdTablePrefix = "uCommerceDataProvider";
		public const string SitecoreDomainName = "sitecore";
		public const string SitecoreProfilesPath = "/sitecore/system/Settings/Security/Profiles";
		public const string SitecoreCoreDatabaseName = "core";
		public const string SitecoreMasterDatabaseName = "master";
		public const string SitecoreWebDatabaseName = "web";
		public const string SitecoreStatisticsSectionId = "1597272C-C823-4AAC-86F8-CA9CC4D573B5";

		public const string UCommerceDynamicTemplatesFolderName = "uCommerce definitions";
		public const string UCommerceIconFolder = "/sitecore%20modules/Shell/ucommerce/images";

		public const string FieldTypeText = "text";
		public const string FieldTypeShortText = "Single-Line Text";
		public const string FieldTypeRichText = "Rich Text";
		public const string FieldTypeDatetime = "Datetime";
		public const string FieldTypeBoolean = "Checkbox";
		public const string FieldTypeImage = "Image";
		public const string FieldTypeNumber = "Number";
		public const string FieldTypeDropdown = "Droplink";
		public const string FieldTypeCheckboxList = "Checklist";

		// Original Sitecore tree lists.
		public const string FieldTypeTreeList = "Treelist";
		public const string FieldTypeTreeListWithSearch = "Treelist with Search";
		public const string FieldTypeMultiListWithSearch = "Multilist with Search";

		// uCommerce specific tree lists. Used for better performance.
		public const string FieldTypeProductsTreeListWithSearch = "Products Multilist with Search";
		public const string FieldTypeCategoriesTreeList = "Categories Treelist";

		public const string SitecoreDataProviderTreeServiceId = "TreeServiceContentEditor";

	    public const string RewriteCategoryProduct = "categoryProduct";
        public const string RewriteProduct = "product";
        public const string RewriteCategory = "category";
        public const string RewriteCatalog = "catalog";

		public const bool EnableCacheable = false;

		public static readonly Guid DomainItemBaseId = new Guid("{6575AF58-6D71-4F44-9DB4-E5B65A305584}");
		public static readonly Guid MemberGroupItemBaseId = new Guid("{F0A64FD6-BCC7-4FEB-8DB9-A0607605E0DB}");
		public static readonly Guid MemberTypeItemBaseId = new Guid("{6D17107F-0A20-40E6-B802-96A5517D2244}");
    }
}
