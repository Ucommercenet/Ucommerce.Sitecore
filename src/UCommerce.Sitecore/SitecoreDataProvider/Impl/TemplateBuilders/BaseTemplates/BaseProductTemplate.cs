using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;
using UCommerce.Sitecore.Settings;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class BaseProductTemplate
	{
        private static bool? IncludeProductsCategoryRelations { get; set; }
        private static bool? IncludeTheProductsProductRelations { get; set; }

        private Dictionary<int, PriceGroupFieldData> _priceGroupFields;

		public TemplateItem BuildBaseProductTemplate()
		{
			var builder = new TemplateBuilder();
			_priceGroupFields = new Dictionary<int, PriceGroupFieldData>();
			
			builder.CreateTemplate("ProductBaseTemplate", FieldIds.Product.ProductBaseTemplateId, "Product Base Template", TemplateIDs.StandardTemplate);

			BuildCommonSection(builder, 100);
			BuildCategoriesSection(builder, 200);
			BuildRelatedProductsSection(builder, 400);
			BuildPriceGroupSections(builder, 300);
			BuildMediaSection(builder, 500);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");

			return template;
		}

		public string GetCommaSeperatedProductTemplatesList()
		{
			var productDefinitions = ProductDefinition.All().Select(x => x.Name).ToList();
			return productDefinitions.Any() 
				? productDefinitions.Aggregate((x, y) => x + "," + y)
				: "";
		}

		public string GetCommaSeperatedProductVariantTemplatesList()
		{
			var productDefinitions = ProductDefinition.All().Select(x => x.Name).ToList();
			return productDefinitions.Any()
				? productDefinitions.Aggregate((x, y) => x + " Variant," + y + " Variant")
				: "";
		}

		public string GetCommaSpereatedCategoryTemplatesList()
		{
			var definitions = Definition.Find(x => x.DefinitionType.DefinitionTypeId == 1).Select(x => x.Name).ToList();

			return definitions.Any()
				? definitions.Aggregate((x, y) => x + "," + y)
				: "";
		}

		public int GetPriceGroupId(ID priceFieldId)
		{
			return _priceGroupFields.SingleOrDefault(x => x.Value.PriceFieldId == priceFieldId).Key;
		}

		private void BuildRelatedProductsSection(TemplateBuilder builder, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Relations"), FieldIds.Product.SectionRelatedProductsId,sortOrder);

			var dataSource = string.Format("StartSearchLocation={0}", FieldIds.Product.ProductsRootFolderId);
			builder.CreateTreeListWithSearchField(FieldIds.Product.ProductRelations, "Related products", 1, dataSource, new List<KeyValuePair<string, string>>());
		}

		private void BuildCategoriesSection(TemplateBuilder builder, int sortorder)
		{
			var definitions = GetCommaSeperatedProductTemplatesList();
			var variantDefinitions = GetCommaSeperatedProductVariantTemplatesList();
			var excludeFromDisplay = definitions + "," + variantDefinitions;
			
			var categoryDefinitions = Definition.All().Where(x => x.DefinitionType.DefinitionTypeId == 1).Select(x => x.Name).ToList();
		
			var categoryDefinitionsString = string.Empty;
			if (categoryDefinitions.Any())
				categoryDefinitionsString = categoryDefinitions.Aggregate((x, y) => x + "," + y);
			
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Catalogs"), FieldIds.Product.SectionCategoriesId, sortorder);

			// Defining the datasource for the tree list
			var datasource = "DataSource=/sitecore/uCommerce/*&IncludeTemplatesForSelection={0}&ExcludeTemplatesForDisplay={1}"
				.FormatWith(categoryDefinitionsString, excludeFromDisplay);

			builder.CreateCategoriesTreeListField(FieldIds.Product.CategoriesTreeListId, "Categories list", 1, datasource, new List<KeyValuePair<string, string>>());
		}

		public void AddBaseFieldValuesForProduct(Product product, FieldList list, VersionUri version)
		{
			AddCommonFieldsForProduct(product, list, version);

            // NOTE: These "cost" a lot to give to Sitecore
            // because it needs to check the relationships as
            // the items are published. So we leave them out of
            // of the publish/index process by deafult.

            // The behaviour can be controlled using the Settings object.
            if (IncludeProductsCategoryRelations ?? ReadIncludeProductsCategoryRelationsFromSettings())
            {
                AddCategoriesFieldsForProduct(product, list);
            }

            if (IncludeTheProductsProductRelations ?? ReadIncludeTheProductsProductRelationsFromSettings())
            { 
				AddRelatedProductsFieldsForProduct(product, list);
			}

			AddProductPriceFieldsForPriceGroups(product, list);
			AddMediaFieldsForProduct(product, list);
			AddAuditFieldsForProduct(product, list);
		}

        private bool ReadIncludeProductsCategoryRelationsFromSettings()
        {
            var settings = ObjectFactory.Instance.Resolve<DataProviderSettings>();
            bool b = settings.IncludeProductCategoryRelationsData;
            IncludeProductsCategoryRelations = b;

            return b;
        }

        private bool ReadIncludeTheProductsProductRelationsFromSettings()
        {
            var settings = ObjectFactory.Instance.Resolve<DataProviderSettings>();
            bool b = settings.IncludeProductRelationsData;
            IncludeTheProductsProductRelations = b;

            return b;
        }

        private void BuildCommonSection(TemplateBuilder builder, int sortorder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Common"), FieldIds.Product.SectionCommonId, sortorder);

			builder.CreateTextField(FieldIds.Product.SkuFieldId, "SKU", 1);
			builder.CreateTextField(FieldIds.Product.ProductIdFieldId, "Product id", 2);
			builder.CreateTextField(FieldIds.Product.InternalNameFieldId, "Internal name", 3);
			builder.CreateCheckBoxField(FieldIds.Product.DisplayOnSiteFieldId, "Display on site", 4);
			builder.CreateCheckBoxField(FieldIds.Product.AllowOrderingFieldId, "Allow ordering", 5);

			// Multi lingual
			builder.CreateTextField(FieldIds.Product.DisplayNameFieldId, "Display name", 6);
			builder.CreateTextField(FieldIds.Product.ShortDescriptionFieldId, "Short description", 7);
			builder.CreateRichTextEditorField(FieldIds.Product.LongDescriptionFieldId, "Long description", 8);
		}

		private void BuildPriceGroupSections(TemplateBuilder builder, int sortorder)
		{
			// Assumption: Price groups are the same for all the products.
			// So all products should have entries for all the price groups.

			var priceGroups = PriceGroup.Find(x => !x.Deleted).ToList();
			BuildPriceGroupSectionForSpecificPriceGroups(builder, priceGroups, sortorder);
		}

		private void BuildPriceGroupSectionForSpecificPriceGroups(TemplateBuilder builder, IEnumerable<PriceGroup> priceGroups, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			var sectionName = resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Pricing");

			builder.CreateSection(sectionName, FieldIds.Product.SectionPricingId, sortOrder);

			foreach (var priceGroup in priceGroups)
			{
				AddPriceGroupSpecificField(builder, priceGroup);
				sortOrder++;
			}
		}

		private void AddPriceGroupSpecificField(TemplateBuilder builder, PriceGroup priceGroup)
		{
			ID priceFieldId = priceGroup.SitecoreTemplateFieldId(FieldIds.Product.SectionPricingId);

			var priceGroupFieldData = new PriceGroupFieldData { PriceFieldId = priceFieldId };
			_priceGroupFields[priceGroup.PriceGroupId] = priceGroupFieldData;

			builder.CreateNumberField(priceFieldId, GetPriceGroupName(priceGroup), 1, new List<KeyValuePair<string, string>>());
		}

		private string GetPriceGroupName(PriceGroup priceGroup)
		{
			return string.Format("{0} ({1})", priceGroup.Name, priceGroup.Currency.ISOCode);
		}

		private void BuildMediaSection(TemplateBuilder builder, int sortorder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Media"), FieldIds.Product.SectionMediaId, sortorder);

			builder.CreateImageField(FieldIds.Product.ThumbnailFieldId, "Thumbnail image", 1);
			builder.CreateImageField(FieldIds.Product.PrimaryImageFieldId, "Primary image", 2);
		}

		private void AddCommonFieldsForProduct(Product product, FieldList list, VersionUri version)
		{
			// Bucketable
			list.SafeAdd(new ID("{C9283D9E-7C29-4419-9C28-5A5C8FF53E84}"), true.ToSitecoreFormat());

			// Common
			list.SafeAdd(FieldIds.Product.SkuFieldId, product.Sku);
			list.SafeAdd(FieldIds.Product.ProductIdFieldId, product.ProductId.ToString());
			list.SafeAdd(FieldIds.Product.InternalNameFieldId, product.Name);
			list.SafeAdd(FieldIds.Product.DisplayOnSiteFieldId, product.DisplayOnSite.ToSitecoreFormat());
			list.SafeAdd(FieldIds.Product.AllowOrderingFieldId, product.AllowOrdering.ToSitecoreFormat());

			// Multi-lingual
			var description = product.ProductDescriptions.FirstOrDefault(x => x.CultureCode == version.Language.CultureInfo.Name);
			if (description != null && !string.IsNullOrEmpty(description.DisplayName))
			{
				list.SafeAdd(FieldIds.Product.DisplayNameFieldId, description.DisplayName);
			}

			if (description != null && !string.IsNullOrEmpty(description.ShortDescription))
			{
				list.SafeAdd(FieldIds.Product.ShortDescriptionFieldId, description.ShortDescription);
			}

			if (description != null && !string.IsNullOrEmpty(description.LongDescription))
			{
				list.SafeAdd(FieldIds.Product.LongDescriptionFieldId, description.LongDescription);
			}
		}

		private void AddCategoriesFieldsForProduct(Product product, FieldList list)
		{
			// The guids must use "B" ToString, to get the curly braces around the ID. 
			// If this is not done, the indexing task will not be able to properly parse the IDs.
			IEnumerable<string> categoryIds = product.CategoryProductRelations
				.Where(x => !x.Category.Deleted)
				.Select(x => x.Category.Guid.ToString("B"))
				.ToList();

			list.SafeAdd(FieldIds.Product.CategoriesTreeListId, string.Join("|", categoryIds));
		}

		private void AddRelatedProductsFieldsForProduct(Product product, FieldList list)
		{
			// The guids must use "B" ToString, to get the curly braces around the ID. 
			// If this is not done, the indexing task will not be able to properly parse the IDs.
			IEnumerable<string> productIds = product.ProductRelations
				.Select(x => x.RelatedProduct.Guid.ToString("B"))
				.ToList();

			list.SafeAdd(FieldIds.Product.ProductRelations, string.Join("|", productIds));
		}

		private void AddProductPriceFieldsForPriceGroups(Product product, FieldList list)
		{
            var priceGroups = PriceGroup.Find(x=> !x.Deleted).ToList();
		    var productPriceRepository = ObjectFactory.Instance.Resolve<IRepository<ProductPrice>>();
		    var userServie = ObjectFactory.Instance.Resolve<IUserService>();

		    var productPrices = productPriceRepository.Select(x => x.Product == product && x.MinimumQuantity == 1);

            foreach (var priceGroup in priceGroups)
            {
                var singlePriceForProductAndPriceGroup = productPrices.FirstOrDefault(x => x.Price.PriceGroup == priceGroup);
                
                AddProductPriceFieldAndValue(singlePriceForProductAndPriceGroup, list, userServie.GetCurrentUserCulture().NumberFormat);
            }
        }

        private void AddProductPriceFieldAndValue(ProductPrice productPrice, FieldList list, NumberFormatInfo numberFormat)
		{
			if (productPrice == null || !_priceGroupFields.ContainsKey(productPrice.Price.PriceGroup.PriceGroupId)) return;

			PriceGroupFieldData fieldsIds = _priceGroupFields[productPrice.Price.PriceGroup.PriceGroupId];

            list.SafeAdd(fieldsIds.PriceFieldId, GetFormattedPriceValueFromProductPrice(productPrice, numberFormat));
		}

	    private string GetFormattedPriceValueFromProductPrice(ProductPrice productPrice, NumberFormatInfo numberFormat)
	    {
	        if (productPrice == null)
	        {
	            return "0";
	        }

	        return productPrice.Price.Amount.ToString(numberFormat);
	    }

	    private void AddMediaFieldsForProduct(Product product, FieldList list)
		{
			// Media
			if (!string.IsNullOrEmpty(product.ThumbnailImageMediaId))
			{
				list.SafeAdd(FieldIds.Product.ThumbnailFieldId, string.Format("<image mediaid=\"{0}\" />", product.ThumbnailImageMediaId));
			}
			if (!string.IsNullOrEmpty(product.PrimaryImageMediaId))
			{
				list.SafeAdd(FieldIds.Product.PrimaryImageFieldId, string.Format("<image mediaid=\"{0}\" />", product.PrimaryImageMediaId));
			}
		}

		private void AddAuditFieldsForProduct(Product product, FieldList list)
		{
			list.SafeAdd(FieldIDs.Created, product.CreatedOn.ToSitecoreFormat());
			list.SafeAdd(FieldIDs.CreatedBy, product.CreatedBy);
			list.SafeAdd(FieldIDs.Updated, product.ModifiedOn.ToSitecoreFormat());
			list.SafeAdd(FieldIDs.UpdatedBy, product.ModifiedBy);
			list.SafeAdd(FieldIDs.Revision, product.Guid.Derived(product.ModifiedOn).ToString());
		}

		private class PriceGroupFieldData
		{
			public ID PriceFieldId { get; set; }
		}
	}
}
