using System;
using System.Collections.Generic;
using Sitecore.Data;
using UCommerce.EntitiesV2;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;
using UCommerce.Sitecore.Extensions;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class TemplateBuilder
	{
		private TemplateItem _currentTemplatePiece;
		private SectionItem _currentSectionPiece;

		private bool _readyForTemplate;
		private bool _readyForSection;
		private bool _readyForField;

		public TemplateBuilder()
		{
			_readyForTemplate = true;
			_readyForSection = false;
			_readyForField = false;
		}

		public TemplateItem Build()
		{
			_readyForTemplate = false;
			_readyForSection = false;
			_readyForField = false;

			return _currentTemplatePiece;
		}

		public void CreateTemplate(string name, ID templateId, string fullName, ID baseTemplateId)
		{
			CreateTemplate(name, templateId, fullName, new List<ID> { baseTemplateId} );
		}

		public void CreateTemplate(string name, ID templateId, string fullName, IEnumerable<ID> baseTemplateIds)
		{
			if (!_readyForTemplate) { throw new InvalidOperationException(); }
			_readyForTemplate = false;
			_readyForSection = true;

			_currentTemplatePiece = new TemplateItem(templateId, name, baseTemplateIds);
			_currentTemplatePiece.AddStandardValues(new StandardValuesItem(templateId));
		}

		public void CreateSection(string name, ID sectionId, int sortOrder)
		{
			if (!_readyForSection) { throw new InvalidOperationException(); }
			_readyForField = true;

			_currentSectionPiece = new SectionItem(sectionId, name, sortOrder);
			_currentTemplatePiece.AddSection(_currentSectionPiece);
		}

		private void CreateField(ID fieldId,
			string fieldName,
			string icon,
			bool shared,
			int sortOrder,
			string style,
			string type,
			bool unversioned,
			List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, icon, shared, sortOrder, style, type, unversioned, string.Empty, displayNamesByCultureCodes);
		}

		private void CreateField(ID fieldId,
		                           string fieldName,
		                           string icon,
		                           bool shared,
		                           int sortOrder,
		                           string style,
		                           string type,
		                           bool unversioned,
		                           string source,
		                           List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			if (!_readyForField) { throw new InvalidOperationException(); }

			_currentSectionPiece.AddField(new FieldItem(fieldId, fieldName, icon, sortOrder, style, type, shared, unversioned, source, displayNamesByCultureCodes));
		}

		private void CreateField(ID fieldId,
								   string fieldName,
								   string icon,
								   bool shared,
								   int sortOrder,
								   string style,
								   string type,
								   bool unversioned,
								   string source,
								   List<KeyValuePair<string, string>> displayNamesByCultureCodes,
								   string validation,
								   string validationText)
		{
			if (!_readyForField) { throw new InvalidOperationException(); }

			var fieldItem = new FieldItem(fieldId, fieldName, icon, sortOrder, style, type, shared, unversioned, source,
				displayNamesByCultureCodes);

			fieldItem.Validation = validation;
			fieldItem.ValidationText = validationText;

			_currentSectionPiece.AddField(fieldItem);
		}

		public void CreateShortTextField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateShortTextField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateShortTextField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeShortText, true, displayNamesByCultureCodes);
		}

		public void CreateTextField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateTextField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateTextField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeText, true, displayNamesByCultureCodes);
		}

		public void CreateRichTextEditorField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateRichTextEditorField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateRichTextEditorField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeRichText, true, displayNamesByCultureCodes);
		}

		public void CreateDateTimeField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateDateTimeField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateDateTimeField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeDatetime, true, displayNamesByCultureCodes);
		}

		public void CreateCheckBoxField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateCheckBoxField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateCheckBoxField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeBoolean, true, displayNamesByCultureCodes);
		}

		public void CreateImageField(ID fieldId, string fieldName, int sortOrder)
		{
			CreateImageField(fieldId, fieldName, sortOrder, new List<KeyValuePair<string, string>>());
		}

		public void CreateImageField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeImage, true, displayNamesByCultureCodes);
		}

		public void CreateNumberField(ID fieldId, string fieldName, int sortOrder, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeNumber, true, displayNamesByCultureCodes);
		}

		public void CreateTreeListField(ID fieldId, string fieldName, int sortOrder, string source, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeMultiListWithSearch, true, source, displayNamesByCultureCodes, @"^(\{[^}]+\}\|?){0,1}$", "You can select a maximum of one content item.");
		}

		public void CreateCategoriesTreeListField(ID fieldId, string fieldName, int sortOrder, string source, List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeCategoriesTreeList, true, source, displayNamesByCultureCodes);
		}

		public void CreateTreeListWithSearchField(ID fieldId, string fieldName, int sortOrder, string source,
			List<KeyValuePair<string, string>> displayNamesByCultureCodes)
		{
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeProductsTreeListWithSearch, true, source, displayNamesByCultureCodes);
		}

		public void CreateDropdownList(ID fieldId, string fieldName, int sortOrder, ID sourceId)
		{
			var dataSource = string.Format("DataSource={0}", sourceId);
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeDropdown, true, dataSource, new List<KeyValuePair<string, string>>());
		}

		public void CreateCheckboxList(ID fieldId, string fieldName, int sortOrder, ID sourceId)
		{
			var dataSource = string.Format("DataSource={0}", sourceId);
			CreateField(fieldId, fieldName, string.Empty, false, sortOrder, string.Empty, SitecoreConstants.FieldTypeCheckboxList, true, dataSource, new List<KeyValuePair<string, string>>());
		}

		public void CreateDynamicItemFieldByDataType(ID fieldId, DataType dataType, int sortOrder,
		                                                List<KeyValuePair<string, string>> displayNamesByCultureCodes, string fieldName)
		{
			switch (dataType.DefinitionName)
			{
				case "ShortText":
					CreateShortTextField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "LongText":
					CreateTextField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "Enum":
					CreateDropdownList(fieldId, fieldName, sortOrder, dataType.SitecoreIdForEnum());
					break;
				case "EnumMultiSelect":
					CreateCheckboxList(fieldId, fieldName, sortOrder, dataType.SitecoreIdForEnum());
					break;
				case "Number":
					CreateNumberField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "Boolean":
					CreateCheckBoxField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "Media":
					CreateImageField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "DateTime":
				case "Date":
					CreateDateTimeField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "RichText":
					CreateRichTextEditorField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
				case "Content":
					const string dataSource = "StartSearchLocation={0DE95AE4-41AB-4D01-9EB0-67441B7C2450}";
					CreateTreeListField(fieldId, fieldName, sortOrder, dataSource, displayNamesByCultureCodes);
					break;
				default:
					// As default we create a Short Text field for data types we do not know.
					CreateShortTextField(fieldId, fieldName, sortOrder, displayNamesByCultureCodes);
					break;
			}
		}
	}
}
