using System;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Sitecore.Entities;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.StandardTemplateFields
{
	internal class StandardFieldValuePersistor : ISitecoreFieldValuePersistor
	{
		public FieldDataTransferObject ReadAllFields()
		{
			var result = new FieldDataTransferObject();

			AddSharedFieldsFromRepository(result);
			AddUnversionedFieldsFromRepository(result);
			AddVersionedFieldsFromRepository(result);

			return result;
		}

		private void AddSharedFieldsFromRepository(FieldDataTransferObject result)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<SharedField>>();
			var fields = repository.Select().ToList();

			fields.ForEach(x => result.SharedFields.Add(new SharedFieldDataTransferObject {FieldId = x.FieldId, ItemId = x.ItemId, Value = x.FieldValue} ));
		}

		private void AddUnversionedFieldsFromRepository(FieldDataTransferObject result)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<UnversionedField>>();
			var fields = repository.Select().ToList();

			fields.ForEach(x => result.UnversionedFields.Add(new UnversionedFieldDataTransferObject { FieldId = x.FieldId, ItemId = x.ItemId, Language = x.Language, Value = x.FieldValue }));
		}

		private void AddVersionedFieldsFromRepository(FieldDataTransferObject result)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<VersionedField>>();
			var fields = repository.Select().ToList();

			fields.ForEach(x => result.VersionedFields.Add(new VersionedFieldDataTransferObject { FieldId = x.FieldId, ItemId = x.ItemId, Language = x.Language, Version = x.Version, Value = x.FieldValue }));
		}

		public void AddSharedField(Guid itemId, Guid fieldId, string val)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<SharedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId).FirstOrDefault() ?? new SharedField();

			item.ItemId = itemId;
			item.FieldId = fieldId;
			item.FieldValue = val;

			repository.Save(item);
		}

		public void AddUnversionedField(Guid itemId, Guid fieldId, string val, string language)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<UnversionedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId && x.Language == language).FirstOrDefault() ?? new UnversionedField();

			item.ItemId = itemId;
			item.FieldId = fieldId;
			item.Language = language;
			item.FieldValue = val;

			repository.Save(item);
		}

		public void AddVersionedField(Guid itemId, Guid fieldId, string val, string language, int version)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<VersionedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId && x.Language == language && x.Version == version).FirstOrDefault() ?? new VersionedField();

			item.ItemId = itemId;
			item.FieldId = fieldId;
			item.Language = language;
			item.Version = version;
			item.FieldValue = val;

			repository.Save(item);
		}

		public void DeleteSharedField(Guid itemId, Guid fieldId)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<SharedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId).FirstOrDefault();
			if (item != null)
			{
				repository.Delete(item);
			}
		}

		public void DeleteUnversionedField(Guid itemId, Guid fieldId, string language)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<UnversionedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId && x.Language == language).FirstOrDefault();
			if (item != null)
			{
				repository.Delete(item);
			}
		}

		public void DeleteVersionedField(Guid itemId, Guid fieldId, string language, int version)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<VersionedField>>();

			var item = repository.Select(x => x.ItemId == itemId && x.FieldId == fieldId && x.Language == language && x.Version == version).FirstOrDefault();
			if (item != null)
			{
				repository.Delete(item);
			}
		}

		private class SharedFieldDataTransferObject : ISharedField
		{
			public Guid ItemId { get; set; }
			public Guid FieldId { get; set; }
			public string Value { get; set; }
		}

		private class UnversionedFieldDataTransferObject : SharedFieldDataTransferObject, IUnversionedField
		{
			public string Language { get; set; }
		}

		private class VersionedFieldDataTransferObject : UnversionedFieldDataTransferObject, IVersionedField
		{
			public int Version { get; set; }
		}
	}
}
