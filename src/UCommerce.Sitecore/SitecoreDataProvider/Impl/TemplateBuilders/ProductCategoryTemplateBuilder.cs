using System;
using System.Collections.Generic;
using System.IO;
using Sitecore;
using Sitecore.Data;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Extensions;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Sitecore.Settings;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions;
using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class ProductCategoryTemplateBuilder : ITemplateBuilder
	{
		private readonly ILoggingService _loggingService;
		private readonly DynamicCategoryTemplate _dynamicCategoryTemplate;
	    private readonly DataProviderSettings _dataProviderSettings;

        private bool EnforceUniquenessOfCategoryNames { get; }

		public ProductCategoryTemplateBuilder(ILoggingService loggingService)
		{
			_dynamicCategoryTemplate = new DynamicCategoryTemplate();
			_loggingService = loggingService;
		    _dataProviderSettings = ObjectFactory.Instance.Resolve<DataProviderSettings>();

		    EnforceUniquenessOfCategoryNames = _dataProviderSettings.EnforceUniquenessOfCategoryNames;

        }

        public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var templateData = new List<ISitecoreItem>();

			var builder = new TemplateBuilder();

			builder.CreateTemplate("ProductCategoryTemplate", FieldIds.Category.ProductCategoryTemplateId, "Product Category Template", TemplateIDs.StandardTemplate);

			BuildCommonSection(builder, 100);
			BuildMediaSection(builder, 200);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");
			template.AddToFieldList(FieldIDs.Revision, Guid.NewGuid().ToString());
			templateData.Add(template);

			var categoryDefinitionType = DefinitionType.Get(1);

			var repository = ObjectFactory.Instance.Resolve<IRepository<Definition>>();
			var definitions = repository.Select(x => x.DefinitionType == categoryDefinitionType).ToList();
			foreach (var definition in definitions)
			{
				var definitionTemplate = _dynamicCategoryTemplate.BuildCategoryTemplateFromDefinition(definition);
				definitionTemplate.SetRevision(definition.Guid.Derived(definition.ModifiedOn));
				templateData.Add(definitionTemplate);
			}

			return templateData;
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			if (!changes.HasFieldsChanged) return false;

			var category = Category.Get(int.Parse(node.ItemId));
			if (category == null)
			{
				string message = string.Format("Category with id: {0} not found for ITreeNodeContent.", node.ItemId);
				_loggingService.Log<ProductCatalogGroupTemplateBuilder>(message);
				throw new InvalidDataException(message);
			}

			foreach (FieldChange fieldChange in changes.FieldChanges) UpdateStoreValueFor(fieldChange, category, changes.Item);

			ObjectFactory.Instance.Resolve<IPipeline<Category>>("SaveCategory").Execute(category);

			return true;
		}

		private void UpdateStoreValueFor(FieldChange fieldChange, Category category, Item item)
		{
			if (this.ValueDidNotChangeFor(fieldChange)) return;
			if (this.FieldBelongsToStatistics(fieldChange)) return;

			if (fieldChange.FieldID == FieldIds.Category.DisplayOnSiteFieldId)
				category.DisplayOnSite = (fieldChange.Value == "1");
			else if (fieldChange.FieldID == FieldIds.Category.ImageFieldId)
				category.ImageMediaId = GetImageValue(fieldChange, item);
			else if (fieldChange.FieldID == FieldIds.Category.NameFieldId)
				UpdateCategoryName(fieldChange.Value, category);
			else if (fieldChange.FieldID == FieldIds.Category.DescriptionFieldId)
				UpdateCategoryDescription(fieldChange, category);
			else if (fieldChange.FieldID == FieldIds.Category.DisplayNameFieldId)
				UpdateCategoryDisplayName(fieldChange, category);
			else if (fieldChange.FieldID == FieldIds.Category.ProductsInCategory)
				UpdateCategoryProducts(fieldChange, category);
			else if (fieldChange.FieldID == FieldIds.Category.ParentCatalogIdFieldId)
				return;
			else if (fieldChange.FieldID == FieldIds.Category.CategoryIdFieldId)
				return;
			else if (fieldChange.Definition.Template.ID == _dynamicCategoryTemplate.GetTemplateId(category.Definition.Id))
				UpdateDynamicCategoryProperty(fieldChange, category, item);
			else
			{
				_loggingService.Log<ProductCategoryTemplateBuilder>(
					string.Format("Could not find property: {0} for category: {1}.", fieldChange.Definition.Key, category.Name));
			}
		}

		private void UpdateCategoryProducts(FieldChange fieldChange, Category category)
		{
			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();

			var selectedSitecoreProductIds = fieldChange.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

			var products = productRepository.Select(new ProductsByGuidsQuery(selectedSitecoreProductIds.Select(Guid.Parse).ToList()));

			var relationsToRemove = category.CategoryProductRelations.Where(x => !products.Contains(x.Product)).ToList();

			var productsToAdd = new List<Product>();
			
			foreach (var product in products)
			{
				if (category.CategoryProductRelations.Any(x => x.Product == product)) continue;

				productsToAdd.Add(product);
			}

			foreach (var categoryProductRelation in relationsToRemove)
			{
				category.CategoryProductRelations.Remove(categoryProductRelation);
			}

			foreach (var product in productsToAdd)
			{
				category.AddProduct(product, 0);
			}
		}

		private string GetImageValue(FieldChange fieldChange, Item item)
		{
			var fieldId = fieldChange.FieldID;
			ImageField field = new Field(fieldId, item);
			return field.MediaID.ToString() == "{00000000-0000-0000-0000-000000000000}"
				? null
				: field.MediaID.ToString();
		}

		private void UpdateDynamicCategoryProperty(FieldChange fieldChange, Category category, Item item)
		{
			var property = GetDynamicProperty(fieldChange, category);

			if (property == null) throw new InvalidDataException(string.Format("Tried to modify category property with name: {0} that doesn't exists.", fieldChange.Definition.Key));

			property.SetValue(this.GetFieldSpecificValue(fieldChange,item));
		}

		private static IProperty GetDynamicProperty(FieldChange fieldChange, Category category)
		{
			IProperty property;
			var definition =
				DefinitionField.SingleOrDefault(
					x => x.Name == fieldChange.Definition.Name && x.Definition.DefinitionType.DefinitionTypeId == 1);

			property = definition.Multilingual
				? category.GetProperty(fieldChange.Definition.Name, fieldChange.Language.CultureInfo.ToString())
				: category.GetProperty(fieldChange.Definition.Name);
			return property;
		}

		private CategoryDescription GetCategoryDescriptionByLanguage(FieldChange fieldChange, Category category)
		{
			return category.GetDescription(fieldChange.Language.CultureInfo.ToString());
		}

		private void UpdateCategoryDisplayName(FieldChange fieldChange, Category category)
		{
			var categoryDescription = GetCategoryDescriptionByLanguage(fieldChange, category);
			categoryDescription.DisplayName = fieldChange.Value;
		}

		private void UpdateCategoryDescription(FieldChange fieldChange, Category category)
		{
			var categoryDescription = GetCategoryDescriptionByLanguage(fieldChange, category);
			categoryDescription.Description = fieldChange.Value;
		}

		private void UpdateCategoryName(string value, Category category)
		{
			if (EnforceUniquenessOfCategoryNames)
			{
				var existingCategory = Category.FirstOrDefault(x => x.Name == value && x.ProductCatalog.ProductCatalogId == category.ProductCatalog.ProductCatalogId);
				if (existingCategory != null) return;
			}

			category.Name = value;
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == Constants.DataProvider.NodeType.ProductCategory;
		}

		public ID GetTemplateId(ITreeNodeContent node)
		{
			var mapper = ObjectFactory.Instance.Resolve<ICategoryIdToCategoryDefinitionIdMapperService>();

			var categoryDefinitionId = mapper.MapToCategoryDefinitionId(int.Parse(node.ItemId));

			return _dynamicCategoryTemplate.GetTemplateId(categoryDefinitionId);	
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			int categoryId = int.Parse(node.ItemId);
			AddDataFromCategory(categoryId, list, version);
		}

		private void BuildMediaSection(TemplateBuilder builder, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Media"), FieldIds.Category.SectionMediaId, sortOrder);

			builder.CreateImageField(FieldIds.Category.ImageFieldId, "Image", 0);
		}

		private void BuildCommonSection(TemplateBuilder builder, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Common"), FieldIds.Category.SectionCommonId, sortOrder);

			builder.CreateTextField(FieldIds.Category.NameFieldId, "Name", 1);
			builder.CreateTextField(FieldIds.Category.CategoryIdFieldId, "Category id", 2);
			builder.CreateTextField(FieldIds.Category.ParentCatalogIdFieldId, "Product catalog id", 3);
			builder.CreateCheckBoxField(FieldIds.Category.DisplayOnSiteFieldId, "Display on site", 6);
			builder.CreateTextField(FieldIds.Category.DisplayNameFieldId, "Display name", 7);
			builder.CreateRichTextEditorField(FieldIds.Category.DescriptionFieldId, "Description", 8);

            // Avoid creating products field on categories if the product data is not included.
		    var includeProductData = _dataProviderSettings.IncludeProductData;
            if (includeProductData) { 
			    BuildListOfProductsField(builder, 10);
            }
        }

		private void BuildListOfProductsField(TemplateBuilder builder, int sortOrder)
		{
			var dataSource = string.Format("StartSearchLocation={0}", FieldIds.Product.ProductsRootFolderId);
			builder.CreateTreeListWithSearchField(FieldIds.Category.ProductsInCategory, "Products", sortOrder, dataSource, new List<KeyValuePair<string, string>>());
		}

		protected void AddDataFromCategory(int categoryId, FieldList list, VersionUri version)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<Category>>();
			var category = repository.Select(new SingleCategoryQuery(categoryId)).Single();
			AddDataFromCategory(list, version, category);
		}

		public void AddDataFromCategory(FieldList list, VersionUri version, Category category)
		{
			list.SafeAdd(FieldIds.Category.NameFieldId, category.Name);
            list.SafeAdd(FieldIds.Category.CategoryIdFieldId, category.CategoryId.ToString());
			list.SafeAdd(FieldIds.Category.ParentCatalogIdFieldId, category.ProductCatalog.ProductCatalogId.ToString());
			list.SafeAdd(FieldIds.Category.DisplayOnSiteFieldId, category.DisplayOnSite.ToSitecoreFormat());
			var description = category.GetDescription(version.Language.Name);
			list.SafeAdd(FieldIds.Category.DisplayNameFieldId, description.DisplayName);
			list.SafeAdd(FieldIds.Category.DescriptionFieldId, description.Description);
			list.SafeAdd(FieldIds.Category.ImageFieldId,
				string.IsNullOrEmpty(category.ImageMediaId)
					? string.Empty
					: string.Format("<image mediaid=\"{0}\" />", category.ImageMediaId));

			list.SafeAdd(FieldIDs.Created, category.CreatedOn.ToSitecoreFormat());
			list.SafeAdd(FieldIDs.CreatedBy, category.CreatedBy);
			list.SafeAdd(FieldIDs.Updated, category.ModifiedOn.ToSitecoreFormat());
			list.SafeAdd(FieldIDs.UpdatedBy, category.ModifiedBy);
			list.SafeAdd(FieldIDs.Revision, category.Guid.Derived(category.ModifiedOn).ToString());

		    // Avoid adding product data to categories if the product data is not included.
		    var includeProductData = _dataProviderSettings.IncludeProductData;
		    if (includeProductData)
		    {
		        string productIdList = BuildListOfProductIds(category);
		        list.SafeAdd(FieldIds.Category.ProductsInCategory, productIdList);
		    }

		    _dynamicCategoryTemplate.AddDynamicFieldValuesForCategory(category, list, version);
		}

		private string BuildListOfProductIds(Category category)
		{
			// The guids must use "B" ToString, to get the curly braces around the ID. 
			// If this is not done, the indexing task will not be able to properly parse the IDs.
			var ids = category.CategoryProductRelations.Select(relation => relation.Product.Guid.ToString("B")).ToList();
			return string.Join("|", ids);
		}
	}
}
