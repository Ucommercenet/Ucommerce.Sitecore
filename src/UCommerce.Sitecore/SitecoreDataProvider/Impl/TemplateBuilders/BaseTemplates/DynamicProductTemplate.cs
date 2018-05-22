using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class DynamicProductTemplate
	{
		private Dictionary<int, ID> _productDefinitionIdToTemplateIdMap;
		private Dictionary<int, ID> _productDefinitionFieldIdToFieldIdMap;
		
		private const string _iconFolder = SitecoreConstants.UCommerceIconFolder;

		public DynamicProductTemplate()
		{
			SetupInternalFields();
		}

		private void SetupInternalFields()
		{
			_productDefinitionIdToTemplateIdMap = new Dictionary<int, ID>();
			_productDefinitionFieldIdToFieldIdMap = new Dictionary<int, ID>();
		}

		public TemplateItem BuildProductTemplateFromDefinition(ProductDefinition definition)
		{
			var builder = new TemplateBuilder();

			ID templateId = definition.SitecoreTemplateId();
			builder.CreateTemplate(definition.Name, templateId, definition.Name, definition.GetSitecoreBaseTemplateIds());
			_productDefinitionIdToTemplateIdMap[definition.ProductDefinitionId] = templateId;

			var definitionFields = definition.ProductDefinitionFields.Where(x => x.RenderInEditor && !x.Deleted && !x.IsVariantProperty).ToList();

			if (definitionFields.Any())
			{
				ID sectionId = definition.SitecoreTemplateSectionDynamicDefinitions(templateId);
				var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
				var userService = ObjectFactory.Instance.Resolve<IUserService>();
				builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "DynamicFields"), sectionId, 150);

				int sortOrder = 10;
				foreach (var definitionField in definitionFields)
				{
					CreateDynamicItemField(builder, definitionField, sortOrder, sectionId);
					sortOrder += 10;
				}
			}
			var templateItem = builder.Build();
			templateItem.AddToFieldList(FieldIDs.Icon,_iconFolder + "/ui/map.png");
			return templateItem;
		}

		public ID GetTemplateId(int productDefinitionId)
		{
			if (_productDefinitionIdToTemplateIdMap.ContainsKey(productDefinitionId))
			{
				return _productDefinitionIdToTemplateIdMap[productDefinitionId];
			}

			return null;
		}

		private void CreateDynamicItemField(TemplateBuilder builder, ProductDefinitionField definitionField, int sortOrder, ID parentId)
		{
			ID id = definitionField.SitecoreTemplateField(parentId);
			TemplateFieldHelper.CreateField(builder, definitionField, sortOrder, id);
			_productDefinitionFieldIdToFieldIdMap[definitionField.ProductDefinitionFieldId] = id;
		}

		public void AddDynamicFieldValuesForProduct(Product product, FieldList list, VersionUri version)
		{
			var properties = product.GetProperties().ToList();

			properties.Where(x => x.RenderForCulture(version.Language.CultureInfo.ToString()))
				.ToList()
				.ForEach(x => TemplateFieldHelper.AddDynamicFieldValuesForProduct(list, x, _productDefinitionFieldIdToFieldIdMap));
		}
	}
}