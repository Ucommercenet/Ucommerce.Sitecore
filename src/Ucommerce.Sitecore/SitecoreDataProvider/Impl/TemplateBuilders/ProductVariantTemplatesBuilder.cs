using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Tree;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions;
using Convert = System.Convert;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class ProductVariantTemplatesBuilder : CommonProductsAndVariantsTemplatesBuilder, ITemplateBuilder
	{
		private readonly BaseTemplates.BaseVariantTemplate _baseVariantTemplate;
		private readonly BaseTemplates.DynamicVariantTemplate _dynamicVariantTemplate;
		private readonly ILoggingService _loggingService;

		public ProductVariantTemplatesBuilder(ILoggingService loggingService)
		{
			_loggingService = loggingService;
			_baseVariantTemplate = new BaseTemplates.BaseVariantTemplate();
			_dynamicVariantTemplate = new BaseTemplates.DynamicVariantTemplate();

		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var baseTemplate = _baseVariantTemplate.BuildBaseProductTemplate();
			baseTemplate.SetRevision(Guid.NewGuid());

			var result = new List<ISitecoreItem> { baseTemplate };

			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductDefinition>>();
			var definitions = repository.Select();

			foreach (var definition in definitions)
			{
				var template = _dynamicVariantTemplate.BuildProductTemplateFromDefinition(definition);
				template.SetRevision(definition.Guid.Derived(definition.ModifiedOn));
				result.Add(template);
			}

			return result;
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == "productVariant";
		}

		public ID GetTemplateId(ITreeNodeContent node)
		{
			var productDefinitionId = GetProductDefinitionIdFromProductId(int.Parse(node.ItemId));
			ID productTemplateId = _dynamicVariantTemplate.GetTemplateId(productDefinitionId);

			return productTemplateId ?? FieldIds.Product.ProductBaseTemplateId;
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			switch (node.NodeType)
			{
				case "productVariant":
					AddFieldValuesForProductVariant(node, list, version);
					break;
			}
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			if (!changes.HasFieldsChanged) return false;

			var productVariant = Product.Get(Convert.ToInt32(node.ItemId));

			if (productVariant == null)
			{
				var message = string.Format("Product with id: {0} not found for ITreeNodeContent. ", node.ItemId);
				_loggingService.Log<ProductCatalogTemplateBuilder>(message);
				throw new InvalidDataException(message);
			}

			foreach (FieldChange fieldChange in changes.FieldChanges) UpdateVariantValue(fieldChange, productVariant, changes.Item);

			ObjectFactory.Instance.Resolve<IPipeline<Product>>("SaveProduct").Execute(productVariant.ParentProduct);

			return true;
		}

		private void UpdateVariantValue(FieldChange fieldChange, Product productVariant,Item item)
		{
			if (this.FieldBelongsToStatistics(fieldChange)) return;
			if (this.ValueDidNotChangeFor(fieldChange)) return;

			if (fieldChange.FieldID == FieldIds.Variant.SkuFieldId)
				UpdateProductVariantSku(fieldChange, productVariant);
			else if (fieldChange.FieldID == FieldIds.Variant.ProductIdFieldId)
				return;
			else if (fieldChange.FieldID == FieldIds.Variant.InternalNameFieldId)
				productVariant.Name = fieldChange.Value;
			else if (fieldChange.FieldID == FieldIds.Variant.DisplayNameFieldId)
				UpdateProductVariantDisplayName(fieldChange, productVariant);
			else if (fieldChange.FieldID == FieldIds.Variant.ShortDescriptionFieldId)
				UpdateProductVariantShortDescription(fieldChange, productVariant);
			else if (fieldChange.FieldID == FieldIds.Variant.LongDescriptionFieldId)
				UpdateProductVariantLongDescription(fieldChange, productVariant);
			else if (fieldChange.Definition.Section.ID == FieldIds.Variant.SectionPricingId)
				UpdatePricing(fieldChange, productVariant,item);
			else if (IsDynamicField(fieldChange, productVariant))
				UpdateDynamicFields(fieldChange, productVariant, item);
			else
				_loggingService.Log<ProductTemplatesBuilder>(
					string.Format("Could not find property: {0} for product: {1}.", fieldChange.Definition.Key, productVariant.Name));
		}

		private bool IsDynamicField(FieldChange fieldChange, Product productVariant)
		{
			if (!fieldChange.Definition.ID.IsUCommerceDynamicField()) { return false; }

			var definitionField = GetDefinitionField(fieldChange, productVariant);
			return definitionField != null;
		}

		private void UpdateDynamicFields(FieldChange fieldChange, Product productVariant, Item item)
		{
			var definitionField = GetDefinitionField(fieldChange, productVariant);

			if (definitionField != null)
			{
				if (definitionField.Multilingual)
					productVariant.GetProperty(fieldChange.Definition.Name, fieldChange.Language.CultureInfo.ToString())
						.SetValue(this.GetFieldSpecificValue(fieldChange, item));
				else
					productVariant.GetProperty(fieldChange.Definition.Name)
						.SetValue(this.GetFieldSpecificValue(fieldChange, item));
			}
			else
				throw new Exception(string.Format("Product property: {0} not found on product.", fieldChange.Definition.Name));
		}

		private static ProductDefinitionField GetDefinitionField(FieldChange fieldChange, Product productVariant)
		{
			var definition = ProductDefinition.SingleOrDefault(x => x.Name == productVariant.ProductDefinition.Name);
			var definitionField =
				definition.GetDefinitionFields()
					.Cast<ProductDefinitionField>()
					.SingleOrDefault(x => x.Name == fieldChange.Definition.Name && x.IsVariantProperty);
			return definitionField;
		}

		private void UpdatePricing(FieldChange fieldChange, Product productVariant, Item item)
		{
			int priceGroupId = _baseVariantTemplate.GetPriceGroupId(fieldChange.FieldID);
			this.UpdateProductPricing(fieldChange,item,productVariant,priceGroupId);
		}

		private void UpdateProductVariantLongDescription(FieldChange fieldChange, Product productVariant)
		{
			productVariant.GetDescription(fieldChange.Language.CultureInfo.ToString()).LongDescription = fieldChange.Value;
		}

		private void UpdateProductVariantShortDescription(FieldChange fieldChange, Product productVariant)
		{
			productVariant.GetDescription(fieldChange.Language.CultureInfo.ToString()).ShortDescription = fieldChange.Value;
		}

		private void UpdateProductVariantDisplayName(FieldChange fieldChange, Product productVariant)
		{
			productVariant.GetDescription(fieldChange.Language.CultureInfo.ToString()).DisplayName = fieldChange.Value;
		}

		private void UpdateProductVariantSku(FieldChange fieldChange, Product productVariant)
		{
			if (fieldChange.Value.Length > 30) throw new Exception("Variant SKU may only contain 30 characters.");

			var existingProduct = Product.SingleOrDefault(x => x.Sku == productVariant.Sku && x.VariantSku == fieldChange.Value);
			if (existingProduct != null)
				throw new InvalidDataException("Product with same sku and variant sku already exists. Use unique combination of sku and variantsku.");

			productVariant.VariantSku = fieldChange.Value;
		}

		private void AddFieldValuesForProductVariant(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			var product = ReadProductFromDatabase(int.Parse(node.ItemId));

			if (product != null)
				AddFieldValuesForProductVariant(product, list, version);
		}

		public void AddFieldValuesForProductVariant(Product product, FieldList list, VersionUri version)
		{
			_baseVariantTemplate.AddBaseFieldValuesForProduct(product, list, version);
			_dynamicVariantTemplate.AddDynamicFieldValuesForProduct(product, list, version);
		}

		private int GetProductDefinitionIdFromProductId(int productId)
		{
			var mapperService = ObjectFactory.Instance.Resolve<IProductDefinitionIdMapperService>();
			return mapperService.MapToProductDefinitionId(productId);
		}
	}
}
