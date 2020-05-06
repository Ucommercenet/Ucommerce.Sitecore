using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Sitecore.Extensions;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class TemplateFieldHelper
	{
		public static void CreateField(TemplateBuilder builder, ProductDefinitionField definitionField,
			int sortOrder, ID id)
		{
			if (definitionField.DataType == null) return; // Skip creation, if there is no datatype.

			var displaynamesByCultureCodes = new List<KeyValuePair<string, string>>();

			var descriptions = definitionField.ProductDefinitionFieldDescriptions;
			foreach (var description in descriptions.Where(x => x.ProductDefinitionField.RenderInEditor))
			{
				var cultureCode = description.CultureCode;
				if (displaynamesByCultureCodes.Any(x => x.Key == description.CultureCode)) continue;

				var displayName = description.DisplayName;
				displaynamesByCultureCodes.Add(new KeyValuePair<string, string>(cultureCode, displayName));
			}

			builder.CreateDynamicItemFieldByDataType(id, definitionField.DataType, sortOrder, displaynamesByCultureCodes,
				definitionField.Name);
		}

		public static void AddDynamicFieldValuesForProduct(FieldList list, IProperty property, Dictionary<int, ID> productDefinitionFieldIdToFieldIdMap)
		{
			var definitionField = property.GetDefinitionField() as ProductDefinitionField;
			if (productDefinitionFieldIdToFieldIdMap.ContainsKey(definitionField.ProductDefinitionFieldId))
			{
				var sitecoreFieldId = productDefinitionFieldIdToFieldIdMap[definitionField.ProductDefinitionFieldId];
				list.SafeAdd(sitecoreFieldId, property.ToSitecoreFormat());
			}
		}
	}
}
