using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.EntitiesV2.Queries.Catalog;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions;
using Ucommerce.Tree;
using Convert = System.Convert;
namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class ProductTemplatesBuilder : CommonProductsAndVariantsTemplatesBuilder, ITemplateBuilder
	{
		private readonly BaseProductTemplate _baseProductTemplate;
		private readonly DynamicProductTemplate _dynamicProductTemplate;

		private readonly ILoggingService _loggingService;

		public ProductTemplatesBuilder(ILoggingService loggingService)
		{
			_loggingService = loggingService;

			_baseProductTemplate = new BaseProductTemplate();
			_dynamicProductTemplate = new DynamicProductTemplate();
		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var baseTemplate = _baseProductTemplate.BuildBaseProductTemplate();
			baseTemplate.SetRevision(Guid.NewGuid());

			var result = new List<ISitecoreItem> { baseTemplate };

			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductDefinition>>();
			var definitions = repository.Select(new FullProductDefinitionsQuery()).ToList();

			foreach (var definition in definitions)
			{
				var template = _dynamicProductTemplate.BuildProductTemplateFromDefinition(definition);
				template.SetRevision(definition.Guid.Derived(definition.ModifiedOn));
				result.Add(template);
			}

			return result;
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			if (!changes.HasFieldsChanged) return false;

			var product = ReadProductFromDatabase(Convert.ToInt32(node.ItemId));
			if (product == null)
			{
				var message = string.Format("Product with id: {0} not found for ITreeNodeContent.", node.ItemId);
				_loggingService.Log<ProductTemplatesBuilder>(message);
				throw new InvalidDataException(message);
			}

			foreach (FieldChange change in changes.FieldChanges) UpdateProductValuesFor(change, product, changes.Item);

			ObjectFactory.Instance.Resolve<IPipeline<Product>>("SaveProduct").Execute(product);

			return true;
		}

		private void UpdateProductValuesFor(FieldChange fieldChange, Product product, Item item)
		{
			if (this.FieldBelongsToStatistics(fieldChange)) return;
			if (this.ValueDidNotChangeFor(fieldChange)) return;

			if (fieldChange.FieldID == FieldIds.Product.SkuFieldId)
				UpdateProductSku(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.ProductIdFieldId)
				return;
			else if (fieldChange.FieldID == FieldIds.Product.InternalNameFieldId)
				product.Name = fieldChange.Value;
			else if (fieldChange.FieldID == FieldIds.Product.DisplayOnSiteFieldId)
				UpdateDisplayOnSite(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.AllowOrderingFieldId)
				UpdateAllowOrdering(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.DisplayNameFieldId)
				UpdateDisplayName(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.ShortDescriptionFieldId)
				UpdateShortDescription(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.LongDescriptionFieldId)
				UpdateLongDescription(fieldChange, product);
			else if (IsDynamicField(fieldChange, product))
				UpdateDynamicFields(fieldChange, product, item);
			else if (fieldChange.FieldID == FieldIds.Product.CategoriesTreeListId)
				UpdateSelectedCategories(fieldChange, product);
			else if (fieldChange.Definition.Section.ID == FieldIds.Product.SectionPricingId)
				UpdatePricing(fieldChange, product, item);
			else if (fieldChange.FieldID == FieldIds.Product.ProductRelations)
				UpdateRelatedProducts(fieldChange, product);
			else if (fieldChange.FieldID == FieldIds.Product.ThumbnailFieldId)
				product.ThumbnailImageMediaId = this.GetFieldSpecificValue(fieldChange, item);
			else if (fieldChange.FieldID == FieldIds.Product.PrimaryImageFieldId)
				product.PrimaryImageMediaId = this.GetFieldSpecificValue(fieldChange, item);
			else
				_loggingService.Log<ProductTemplatesBuilder>(
					string.Format("Could not find property: {0} for product: {1}.", fieldChange.Definition.Key, product.Name));
		}

		private bool IsDynamicField(FieldChange fieldChange, Product product)
		{
			if (!fieldChange.Definition.ID.IsUCommerceDynamicField()) { return false; }

			var definitionField = GetDefinitionField(fieldChange, product);
			return definitionField != null;
		}

		private void UpdateDisplayOnSite(FieldChange fieldChange, Product product)
		{
			product.DisplayOnSite = fieldChange.Value == "1";
		}

		private void UpdateAllowOrdering(FieldChange fieldChange, Product product)
		{
			product.AllowOrdering = fieldChange.Value == "1";
		}

		private void UpdateProductSku(FieldChange fieldChange, Product product)
		{
			if (fieldChange.Value.Length > 30) throw new Exception("SKU may only contain 30 characters.");

			var existingProduct = Product.SingleOrDefault(x => x.Sku == fieldChange.Value && x.ParentProduct == null);

			if (existingProduct == null)
				product.Sku = fieldChange.Value;
			else
				throw new Exception(string.Format("Product with SKU: {0} already exists.", fieldChange.Value));
		}

		private void UpdateDisplayName(FieldChange fieldChange, Product product)
		{
			var description = product.GetDescription(fieldChange.Language.CultureInfo.ToString());

			if (description != null)
				description.DisplayName = fieldChange.Value;
		}

		private void UpdateShortDescription(FieldChange fieldChange, Product product)
		{
			var description = product.GetDescription(fieldChange.Language.CultureInfo.ToString());

			if (description != null)
				description.ShortDescription = fieldChange.Value;
		}

		private void UpdateLongDescription(FieldChange fieldChange, Product product)
		{
			var description = product.GetDescription(fieldChange.Language.CultureInfo.ToString());

			if (description != null)
				description.LongDescription = fieldChange.Value;
		}

		private void UpdateDynamicFields(FieldChange fieldChange, Product product, Item item)
		{
			var definitionField = GetDefinitionField(fieldChange, product);

			if (definitionField != null)
			{
				if (definitionField.Multilingual)
				{
					product.GetProperty(fieldChange.Definition.Name, fieldChange.Language.CultureInfo.ToString())
						.SetValue(this.GetFieldSpecificValue(fieldChange, item));
				}
				else
				{
					product.GetProperty(fieldChange.Definition.Name)
						.SetValue(this.GetFieldSpecificValue(fieldChange, item));
				}
			}
			else
			{
				throw new Exception(string.Format("Product property: {0} not found on product.", fieldChange.Definition.Name));
			}
		}

		private static IDefinitionField GetDefinitionField(FieldChange fieldChange, Product product)
		{
			var definition = ProductDefinition.SingleOrDefault(x => x.Name == product.ProductDefinition.Name);
			var definitionField = definition.GetDefinitionFields().SingleOrDefault(x => x.Name == fieldChange.Definition.Name);
			return definitionField;
		}

		private void UpdateSelectedCategories(FieldChange fieldChange, Product product)
		{
			var selectedSitecoreCategoryIds = fieldChange.Value.Split('|');
			var categoryRepository = ObjectFactory.Instance.Resolve<IRepository<Category>>();

			var selectedCategories = categoryRepository.Select(
				x => selectedSitecoreCategoryIds.Select(Guid.Parse).Contains(x.Guid));

			var existingRelations = CategoryProductRelation.Find(x => x.Product.ProductId == product.ProductId)
										.Select(x => x.Category.CategoryId).ToList();

			// Removing not selected categories
			var categoryIdsToBeRemoved = existingRelations.Except(selectedCategories.Select(x => x.CategoryId)).ToList();
			var categoriesToBeRemoved = Category.Find(x => categoryIdsToBeRemoved.Contains(x.CategoryId)).ToList();
			foreach (var categoryToBeRemoved in categoriesToBeRemoved)
			{
				_loggingService.Log<ProductTemplatesBuilder>("Removing category " + categoryToBeRemoved.Name);
				product.RemoveCategory(categoryToBeRemoved);
			}

			// Adding new selected categories
			var categoriesToBeAdded = selectedCategories.Where(x => !existingRelations.Contains(x.CategoryId)).ToList();
			foreach (var categoryToBeAdded in categoriesToBeAdded)
			{
				_loggingService.Log<ProductTemplatesBuilder>("Adding category " + categoryToBeAdded.Name);
				categoryToBeAdded.AddProduct(product, 0);
			}
		}

		private void UpdatePricing(FieldChange fieldChange, Product product, Item item)
		{
			var priceGroupId = _baseProductTemplate.GetPriceGroupId(fieldChange.FieldID);
			this.UpdateProductPricing(fieldChange,item,product,priceGroupId);
		}

		private void UpdateRelatedProducts(FieldChange fieldChange, Product product)
		{
			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();

			var selectedSitecoreProductIds = fieldChange.Value.Split(new []{'|'}, StringSplitOptions.RemoveEmptyEntries);

			var selectedRelations = new List<ProductRelation>();

			var defaultProductRelationType = ProductRelationType.SingleOrDefault(x => x.ProductRelationTypeId == 1);

			var products = productRepository.Select(new ProductsByGuidsQuery(selectedSitecoreProductIds.Select(Guid.Parse).ToList()));
			// Creating relations from selected products
			foreach (var selectedProduct in products)
			{
				var relation = new ProductRelation()
				{
					Product = product,
					RelatedProduct = selectedProduct,
					ProductRelationType = defaultProductRelationType
				};

				selectedRelations.Add(relation);
			}


            // Removing relations
		    var relationsToRemove = product.ProductRelations.Where(
                relation => selectedRelations.All(x => x.RelatedProduct.ProductId != relation.RelatedProduct.ProductId)).ToList();
			relationsToRemove.ForEach(x => product.ProductRelations.Remove(x));

		    // Adding new relations
		    var relationsToAdd = selectedRelations.Where(
		            relation => product.ProductRelations.All(x => x.RelatedProduct.ProductId != relation.RelatedProduct.ProductId));
			relationsToAdd.ForEach(x => product.ProductRelations.Add(x));
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == "product";
		}

		public ID GetTemplateId(ITreeNodeContent node)
		{
			var productDefinitionId = GetProductDefinitionIdFromProductId(int.Parse(node.ItemId));
			ID productTemplateId = _dynamicProductTemplate.GetTemplateId(productDefinitionId);

			return productTemplateId ?? FieldIds.Product.ProductBaseTemplateId;
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			switch (node.NodeType)
			{
				case "product":
					AddFieldValuesForProduct(node, list, version);
					break;
			}
		}

		private void AddFieldValuesForProduct(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			var product = ReadProductFromDatabase(int.Parse(node.ItemId));

			if (product != null)
			{
				AddFieldValuesForProduct(product, list, version);
			}
		}

		public void AddFieldValuesForProduct(Product product, FieldList list, VersionUri version)
		{
			_baseProductTemplate.AddBaseFieldValuesForProduct(product, list, version);
			_dynamicProductTemplate.AddDynamicFieldValuesForProduct(product, list, version);
		}

		private int GetProductDefinitionIdFromProductId(int productId)
		{
			var mapperService = ObjectFactory.Instance.Resolve<IProductDefinitionIdMapperService>();
			return mapperService.MapToProductDefinitionId(productId);
		}
	}
}
