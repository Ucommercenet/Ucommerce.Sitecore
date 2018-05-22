using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class DynamicCategoryTemplate
	{
		private readonly IDictionary<int, ID> _definitionIdToTemplateIdMap;
		private readonly IDictionary<int, ID> _definitionFieldIdToFieldIdMap;

		public DynamicCategoryTemplate()
		{
			_definitionFieldIdToFieldIdMap = new Dictionary<int, ID>();
			_definitionIdToTemplateIdMap = new Dictionary<int, ID>();
		}

		public TemplateItem BuildCategoryTemplateFromDefinition(IDefinition definition)
		{
			var builder = new TemplateBuilder();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();

			var cultureInfo = userService.GetCurrentUserCulture();
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var sectionName = resourceManager.GetLocalizedText(cultureInfo, "Tabs", "DynamicFields");

			var specificDefinition = definition as Definition;

			ID templateId = definition.SitecoreTemplateId();
			builder.CreateTemplate(definition.Name, templateId, definition.Name, definition.GetBaseTemplateIds());
			_definitionIdToTemplateIdMap[specificDefinition.DefinitionId] = templateId;

			var combinedFields = new List<IDefinitionField>();
			combinedFields.AddRange(((Definition)definition).DefinitionFields);

			if (combinedFields.Any())
			{
				ID sectionId = definition.SitecoreDynamicFieldsSectionId(templateId);
				builder.CreateSection(sectionName, sectionId, 150);

				int sortOrder = 10;
				foreach (var definitionField in combinedFields.Where(x => x.RenderInEditor))
				{
					CreateDynamicItemField(builder, definitionField, sortOrder, sectionId);
					sortOrder += 10;
				}
			}

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");

			return template;
		}

		public ID GetTemplateId(int categoryDefinitionId)
		{
			if (_definitionIdToTemplateIdMap.ContainsKey(categoryDefinitionId))
			{
				return _definitionIdToTemplateIdMap[categoryDefinitionId];
			}

			throw new InvalidDataException(string.Format("Template with categoryDefinitionID: {0} not found. Definitions are soft deleted. Ensure all definitions get a template.", categoryDefinitionId));
		}

		private void CreateDynamicItemField(TemplateBuilder builder, IDefinitionField definitionField, int sortOrder, ID parentId)
		{
			if (definitionField.DataType == null) return; // Skip creation, if there is no datatype.
			
			var specificDefinitionField = definitionField as DefinitionField;
			ID id = specificDefinitionField.GetOrCreateSitecoreDynamicFieldFieldId(parentId);
			var displayNamesByCultureCodes = new List<KeyValuePair<string, string>>();

			var descriptions = specificDefinitionField.DefinitionFieldDescriptions;
			foreach (var description in descriptions)
			{
				var cultureCode = description.CultureCode;
				if (displayNamesByCultureCodes.Any(x => x.Key == cultureCode)) continue;

				string displayName = description.DisplayName;
				displayNamesByCultureCodes.Add(new KeyValuePair<string, string>(cultureCode, displayName));
			}

			builder.CreateDynamicItemFieldByDataType(id, definitionField.DataType, sortOrder, displayNamesByCultureCodes, definitionField.Name);

			_definitionFieldIdToFieldIdMap[specificDefinitionField.Id] = id;
		}

		internal void AddDynamicFieldValuesForCategory(Category category, FieldList list, VersionUri version)
		{
		    var multiLingualProperties = category.GetProperties(version.Language.CultureInfo.ToString());

            var properties = category.GetProperties();

			foreach (var property in properties.Where(
				x => (x.CultureCode == null || x.CultureCode == version.Language.CultureInfo.ToString()) && x.GetDefinitionField().RenderInEditor))
				AddDynamicFieldValueForCategory(list, property);

		    foreach (var multiLingualProperty in multiLingualProperties.Where(x => x.GetDefinitionField().RenderInEditor))
		    {
				AddDynamicFieldValueForCategory(list, multiLingualProperty);
            }
        }

		private void AddDynamicFieldValueForCategory(FieldList list, IProperty property)
		{
			var definitionField = property.GetDefinitionField() as DefinitionField;
			if (_definitionFieldIdToFieldIdMap.ContainsKey(definitionField.Id))
			{
				var sitecoreFieldId = _definitionFieldIdToFieldIdMap[definitionField.Id];
				list.SafeAdd(sitecoreFieldId, property.ToSitecoreFormat());
			}
		}
	}
}
