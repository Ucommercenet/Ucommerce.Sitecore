using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Security;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class BaseVariantTemplate
	{

		public static ID VariantBaseTemplateId { get { return ID.Parse("{5CCDE49D-C18F-4DD7-9AA1-948241088A9D}"); } }

		private Dictionary<int, PriceGroupFieldData> _priceGroupFields;

		public TemplateItem BuildBaseProductTemplate()
		{
			var builder = new TemplateBuilder();
			_priceGroupFields = new Dictionary<int, PriceGroupFieldData>();

			builder.CreateTemplate("VariantBaseTemplate", VariantBaseTemplateId, "Variant Base Template", TemplateIDs.StandardTemplate);

			BuildCommonSection(builder, 100);
			BuildPriceGroupSections(builder, 300);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");

			return template;
		}

		public int GetPriceGroupId(ID priceFieldId)
		{
			return _priceGroupFields.First(x => x.Value.PriceFieldId == priceFieldId).Key;
		}

		public void AddBaseFieldValuesForProduct(Product product, FieldList list, VersionUri version)
		{
			AddCommonFieldsForProduct(product, list, version);
			AddPriceGroupFieldsForProduct(product, list);
			AddAuditFieldsForProduct(product, list);
		}

		private void BuildCommonSection(TemplateBuilder builder, int sortorder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Common"),
				FieldIds.Variant.SectionCommonId, sortorder);

			builder.CreateTextField(FieldIds.Variant.SkuFieldId, "SKU", 1);
			builder.CreateTextField(FieldIds.Variant.ProductIdFieldId, "Product id", 2);
			builder.CreateTextField(FieldIds.Variant.InternalNameFieldId, "Internal name", 3);

			// Multi lingual
			builder.CreateTextField(FieldIds.Variant.DisplayNameFieldId, "Display name", 6);
			builder.CreateTextField(FieldIds.Variant.ShortDescriptionFieldId, "Short description", 7);
			builder.CreateRichTextEditorField(FieldIds.Variant.LongDescriptionFieldId, "Long description", 8);
		}

		private void BuildPriceGroupSections(TemplateBuilder builder, int sortorder)
		{
			// Assumption: Price groups are the same for all the products.
			// So all products should have entries for all the price groups.

			var priceGroups = PriceGroup.Find(x => !x.Deleted).ToList();
			BuildPriceGroupSectionForSpecificPriceGroups(builder, priceGroups, sortorder);
		}

		private void BuildPriceGroupSectionForSpecificPriceGroups(TemplateBuilder builder, IList<PriceGroup> priceGroups, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();

			var sectionName = resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Pricing");

			builder.CreateSection(sectionName, FieldIds.Variant.SectionPricingId, sortOrder);

			foreach (var priceGroup in priceGroups)
			{
				AddPriceGroupSpecificField(builder, priceGroup);
				sortOrder++;
			}
		}

		private void AddPriceGroupSpecificField(TemplateBuilder builder, PriceGroup priceGroup)
		{
			ID priceFieldId = priceGroup.SitecoreTemplateFieldIdForVariant(FieldIds.Variant.SectionPricingId);

			var priceGroupFieldData = new PriceGroupFieldData { PriceFieldId = priceFieldId };
			_priceGroupFields[priceGroup.PriceGroupId] = priceGroupFieldData;

			builder.CreateNumberField(priceFieldId, GetPriceGroupName(priceGroup), 1, new List<KeyValuePair<string, string>>());
		}

		private string GetPriceGroupName(PriceGroup priceGroup)
		{
			return string.Format("{0} ({1})", priceGroup.Name, priceGroup.Currency.ISOCode);
		}

		private void AddCommonFieldsForProduct(Product product, FieldList list, VersionUri version)
		{
			// Bucketable
			list.SafeAdd(new ID("{C9283D9E-7C29-4419-9C28-5A5C8FF53E84}"), true.ToSitecoreFormat());

			// Common
			list.SafeAdd(FieldIds.Variant.SkuFieldId, product.VariantSku);
			list.SafeAdd(FieldIds.Variant.ProductIdFieldId, product.ProductId.ToString());
			list.SafeAdd(FieldIds.Variant.InternalNameFieldId, product.Name);

			// Multi-lingual
			var description = product.GetDescription(version.Language.CultureInfo.Name);

			list.SafeAdd(FieldIds.Variant.DisplayNameFieldId, description.DisplayName);

			list.SafeAdd(FieldIds.Variant.ShortDescriptionFieldId, description.ShortDescription);

			list.SafeAdd(FieldIds.Variant.LongDescriptionFieldId, description.LongDescription);
		}

		private void AddPriceGroupFieldsForProduct(Product product, FieldList list)
		{
		    var priceGroups = PriceGroup.Find(x => !x.Deleted).ToList();
		    var productPriceRepository = ObjectFactory.Instance.Resolve<IRepository<ProductPrice>>();
		    var userServie = ObjectFactory.Instance.Resolve<IUserService>();

		    var productPrices = productPriceRepository.Select(x => x.Product == product && x.MinimumQuantity == 1);

            foreach (var priceGroup in priceGroups)
		    {
		        var singlePriceForProductAndPriceGroup = productPrices.FirstOrDefault(x => x.Price.PriceGroup == priceGroup);

                AddProductPriceFieldAndValue(list, singlePriceForProductAndPriceGroup, userServie.GetCurrentUserCulture().NumberFormat);
		    }
        }

	    private void AddProductPriceFieldAndValue(FieldList list, ProductPrice productPrice, NumberFormatInfo numberFormat)
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
