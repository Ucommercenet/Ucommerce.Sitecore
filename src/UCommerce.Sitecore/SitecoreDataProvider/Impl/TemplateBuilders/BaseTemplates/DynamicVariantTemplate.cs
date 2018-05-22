using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;
using UCommerce.Infrastructure;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class DynamicVariantTemplate
	{
		private Dictionary<int, ID> _productDefinitionIdToTemplateIdMap;
		private Dictionary<int, ID> _productDefinitionFieldIdToFieldIdMap;

		public DynamicVariantTemplate()
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
			ID templateId = definition.SitecoreVariantTemplateId();
			builder.CreateTemplate(definition.Name + " Variant", templateId, definition.Name + " Variant", definition.GetSitecoreBaseVariantTemplateIds());

			_productDefinitionIdToTemplateIdMap[definition.ProductDefinitionId] = templateId;

			var combinedFields = definition.ProductDefinitionFields.Where(x => x.IsVariantProperty && !x.Deleted && !x.DataType.Deleted).ToList();

			if (combinedFields.Any())
			{
				ID sectionId = definition.SitecoreTemplateSectionDynamicDefinitionsForVariant(templateId);
				var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
				var userService = ObjectFactory.Instance.Resolve<IUserService>();
				builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "DynamicFields"), sectionId, 150);

				int sortOrder = 10;
				foreach (var definitionField in combinedFields.Where(x => x.RenderInEditor))
				{
					CreateDynamicItemField(builder, definitionField, sortOrder, sectionId);
					sortOrder += 10;
				}
			}

			var template = builder.Build();
			
			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");

			template.SetRevision(definition.Guid.Derived(definition.ModifiedOn));

			return template;
		}

		public ID GetTemplateId(int productId)
		{
			if (_productDefinitionIdToTemplateIdMap.ContainsKey(productId))
			{
				return _productDefinitionIdToTemplateIdMap[productId];
			}

			return null;
		}

		private void CreateDynamicItemField(TemplateBuilder builder, ProductDefinitionField definitionField, int sortOrder, ID parentId)
		{
			ID id = definitionField.SitecoreTemplateFieldForVariant(parentId);
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
