using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Sitecore.Extensions;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.StandardTemplateFields
{
	public class StandardTemplateFieldValueProvider : ISitecoreStandardTemplateFieldValueProvider
	{
		private readonly ISitecoreFieldValuePersistor _persistor;
		private readonly HashSet<ID> _templateIdWhiteList;
		private readonly HashSet<ID> _fieldIdBlackList;

		private readonly Dictionary<Guid, Dictionary<Guid, string>> _sharedFields;
		private readonly Dictionary<Guid, Dictionary<string, Dictionary<Guid, string>>> _unversionedFields;
		private readonly Dictionary<Guid, Dictionary<string, Dictionary<Guid, string>>> _versionedFields;

		public StandardTemplateFieldValueProvider(ISitecoreFieldValuePersistor persistor, IEnumerable<string> templateIdsWhiteList, IEnumerable<string> fieldIdsBlackList)
		{
			_persistor = persistor;

			_sharedFields = new Dictionary<Guid, Dictionary<Guid, string>>();
			_unversionedFields = new Dictionary<Guid, Dictionary<string, Dictionary<Guid, string>>>();
			_versionedFields = new Dictionary<Guid, Dictionary<string, Dictionary<Guid, string>>>();

			_templateIdWhiteList = new HashSet<ID>();

			foreach (var templateId in templateIdsWhiteList)
			{
				_templateIdWhiteList.Add(ID.Parse(templateId));
			}

			_fieldIdBlackList = new HashSet<ID>();
			foreach (var fieldId in fieldIdsBlackList)
			{
				_fieldIdBlackList.Add(ID.Parse(fieldId));
			}

			ReadDataFromPersistor();
		}

		public void AddFieldValues(ID itemId, VersionUri version, FieldList fieldList)
		{
			AddSharedFieldValues(itemId, fieldList);
			AddVersionedFieldValues(itemId, version, fieldList);
			AddUnversionedFieldValues(itemId, version, fieldList);
		}

		public void SaveItem(ID id, ItemChanges changes)
		{
			if (changes.HasFieldsChanged)
			{
				foreach (FieldChange fieldChange in changes.FieldChanges)
				{
					SaveField(id, fieldChange);
				}
			}
		}

		private void SaveField(ID itemId, FieldChange fieldChange)
		{
			if (!IsTemplateOnWhiteList(fieldChange.Definition.Template.ID)) { return; }
			if (IsFieldOnBlackList(fieldChange.FieldID)) { return; }

			if (fieldChange.Definition.IsShared)
			{
				SaveSharedField(itemId, fieldChange);
				return;
			}

			if (fieldChange.Definition.IsUnversioned)
			{
				SaveUnversionedField(itemId, fieldChange);
				return;
			}

			if (fieldChange.Definition.IsVersioned)
			{
				SaveVersionedField(itemId, fieldChange);
			}

			// Ok to fall through, if definition is "unknown" type.
		}

		private bool IsTemplateOnWhiteList(ID templateId)
		{
			return _templateIdWhiteList.Contains(templateId);
		}

		private bool IsFieldOnBlackList(ID fieldId)
		{
			return _fieldIdBlackList.Contains(fieldId);
		}

		private void AddSharedFieldValues(ID itemId, FieldList fieldList)
		{
			if (_sharedFields.ContainsKey(itemId.Guid))
			{
				var values = _sharedFields[itemId.Guid];
				foreach (var pair in values)
				{
					AddKeyValuePairToFieldList(fieldList, pair);
				}
			}
		}

		private void AddKeyValuePairToFieldList(FieldList fieldList, KeyValuePair<Guid, string> pair)
		{
			var id = new ID(pair.Key);
			if (!IsFieldOnBlackList(id))
			{
				fieldList.SafeAdd(id, pair.Value);
			}
		}

		private void AddUnversionedFieldValues(ID itemId, VersionUri version, FieldList fieldList)
		{
			if (_unversionedFields.ContainsKey(itemId.Guid))
			{
				var values = _unversionedFields[itemId.Guid];
				var key = version.Language.CultureInfo.Name;
				AddFieldValue(fieldList, values, key);
			}
		}

		private void AddVersionedFieldValues(ID itemId, VersionUri version, FieldList fieldList)
		{
			if (_versionedFields.ContainsKey(itemId.Guid))
			{
				var values = _versionedFields[itemId.Guid];
				var key = version.Language.CultureInfo.Name + ";" + version.Version.Number;
				AddFieldValue(fieldList, values, key);
			}
		}

		private void AddFieldValue(FieldList fieldList, Dictionary<string, Dictionary<Guid, string>> values, string key)
		{
			if (values.ContainsKey(key))
			{
				var data = values[key];
				foreach (var pair in data)
				{
					AddKeyValuePairToFieldList(fieldList, pair);
				}
			}
		}

		private void SaveSharedField(ID itemId, FieldChange fieldChange)
		{
			if (fieldChange.RemoveField)
			{
				RemoveSharedField(itemId, fieldChange);
				return;
			}

			if (!_sharedFields.ContainsKey(itemId.Guid))
			{
				_sharedFields[itemId.Guid] = new Dictionary<Guid, string>();
			}

			_sharedFields[itemId.Guid][fieldChange.FieldID.Guid] = fieldChange.Value;
			_persistor.AddSharedField(itemId.Guid, fieldChange.FieldID.Guid, fieldChange.Value);
		}

		private void SaveUnversionedField(ID itemId, FieldChange fieldChange)
		{
			if (fieldChange.RemoveField)
			{
				RemoveUnversionedField(itemId, fieldChange);
				return;
			}

			EnsureItemEntry(itemId.Guid, _unversionedFields);

			var values = _unversionedFields[itemId.Guid];
			var key = fieldChange.Language.CultureInfo.Name;
			AddKeyedValue(fieldChange, values, key);

			_persistor.AddUnversionedField(itemId.Guid, fieldChange.FieldID.Guid, fieldChange.Value, fieldChange.Language.CultureInfo.Name);
		}

		private void SaveVersionedField(ID itemId, FieldChange fieldChange)
		{
			if (fieldChange.RemoveField)
			{
				RemoveVersionedField(itemId, fieldChange);
				return;
			}

			EnsureItemEntry(itemId.Guid, _versionedFields);

			var values = _versionedFields[itemId.Guid];
			var key = fieldChange.Language.CultureInfo.Name + ";" + fieldChange.Version.Number;
			AddKeyedValue(fieldChange, values, key);

			_persistor.AddVersionedField(itemId.Guid, fieldChange.FieldID.Guid, fieldChange.Value, fieldChange.Language.CultureInfo.Name, fieldChange.Version.Number);
		}

		private void EnsureItemEntry(Guid itemId, Dictionary<Guid, Dictionary<string, Dictionary<Guid, string>>> fields)
		{
			if (!fields.ContainsKey(itemId))
			{
				fields[itemId] = new Dictionary<string, Dictionary<Guid, string>>();
			}
		}

		private static void AddKeyedValue(FieldChange fieldChange, Dictionary<string, Dictionary<Guid, string>> values, string key)
		{
			if (!values.ContainsKey(key))
			{
				values[key] = new Dictionary<Guid, string>();
			}

			var fieldEntry = values[key];
			fieldEntry[fieldChange.FieldID.Guid] = fieldChange.Value;
		}

		private void RemoveSharedField(ID itemId, FieldChange fieldChange)
		{
			if (!_sharedFields.ContainsKey(itemId.Guid))
			{
				_sharedFields[itemId.Guid] = new Dictionary<Guid, string>();
			}

			var fieldData = _sharedFields[itemId.Guid];
			if (fieldData.ContainsKey(fieldChange.FieldID.Guid))
			{
				fieldData.Remove(fieldChange.FieldID.Guid);
			}

			_persistor.DeleteSharedField(itemId.Guid, fieldChange.FieldID.Guid);
		}

		private void RemoveUnversionedField(ID itemId, FieldChange fieldChange)
		{
			EnsureItemEntry(itemId.Guid, _unversionedFields);
			var fieldData = _unversionedFields[itemId.Guid];
			var key = fieldChange.Language.CultureInfo.Name;

			RemoveFieldDataForKey(fieldChange, fieldData, key);

			_persistor.DeleteUnversionedField(itemId.Guid, fieldChange.FieldID.Guid, fieldChange.Language.CultureInfo.Name);
		}

		private void RemoveVersionedField(ID itemId, FieldChange fieldChange)
		{
			EnsureItemEntry(itemId.Guid, _versionedFields);
			var fieldData = _versionedFields[itemId.Guid];
			var key = fieldChange.Language.CultureInfo.Name + ";" + fieldChange.Version.Number;

			RemoveFieldDataForKey(fieldChange, fieldData, key);

			_persistor.DeleteVersionedField(itemId.Guid, fieldChange.FieldID.Guid, fieldChange.Language.CultureInfo.Name, fieldChange.Version.Number);
		}

		private static void RemoveFieldDataForKey(FieldChange fieldChange, Dictionary<string, Dictionary<Guid, string>> fieldData, string key)
		{
			if (fieldData.ContainsKey(key))
			{
				var fieldDataForKey = fieldData[key];
				if (fieldDataForKey.ContainsKey(fieldChange.FieldID.Guid))
				{
					fieldDataForKey.Remove(fieldChange.FieldID.Guid);
				}
			}
		}

		private void ReadDataFromPersistor()
		{
			var allFields = _persistor.ReadAllFields();

			foreach (var sharedField in allFields.SharedFields)
			{
				CacheSharedField(sharedField);
			}

			foreach (var unversionedField in allFields.UnversionedFields)
			{
				CacheUnversionedField(unversionedField);
			}

			foreach (var versionedField in allFields.VersionedFields)
			{
				CacheVersionedField(versionedField);
			}
		}

		private void CacheSharedField(ISharedField sharedField)
		{
			if (!_sharedFields.ContainsKey(sharedField.ItemId))
			{
				_sharedFields[sharedField.ItemId] = new Dictionary<Guid, string>();
			}

			_sharedFields[sharedField.ItemId][sharedField.FieldId] = sharedField.Value;
		}

		private void CacheUnversionedField(IUnversionedField unversionedField)
		{
			EnsureItemEntry(unversionedField.ItemId, _unversionedFields);

			var entryForItem = _unversionedFields[unversionedField.ItemId];
			var key = unversionedField.Language;
			if (!entryForItem.ContainsKey(key))
			{
				entryForItem[key] = new Dictionary<Guid, string>();
			}

			var entryForItemWithKey = entryForItem[key];
			entryForItemWithKey[unversionedField.FieldId] = unversionedField.Value;
		}

		private void CacheVersionedField(IVersionedField versionedField)
		{
			EnsureItemEntry(versionedField.ItemId, _versionedFields);

			var entryForItem = _versionedFields[versionedField.ItemId];
			var key = versionedField.Language + ";" + versionedField.Version;
			if (!entryForItem.ContainsKey(key))
			{
				entryForItem[key] = new Dictionary<Guid, string>();
			}

			var entryForItemWithKey = entryForItem[key];
			entryForItemWithKey[versionedField.FieldId] = versionedField.Value;
		}

	}
}
