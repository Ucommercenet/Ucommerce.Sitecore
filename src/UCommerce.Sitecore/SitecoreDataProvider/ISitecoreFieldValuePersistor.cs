using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	public interface ISitecoreFieldValuePersistor
	{
		FieldDataTransferObject ReadAllFields();

		void AddSharedField(Guid itemId, Guid fieldId, string val);
		void AddUnversionedField(Guid itemId, Guid fieldId, string val, string language);
		void AddVersionedField(Guid itemId, Guid fieldId, string val, string language, int version);

		void DeleteSharedField(Guid itemId, Guid fieldId);
		void DeleteUnversionedField(Guid itemId, Guid fieldId, string language);
		void DeleteVersionedField(Guid itemId, Guid fieldId, string language, int version);
	}

	public class FieldDataTransferObject
	{
		public FieldDataTransferObject()
		{
			SharedFields = new List<ISharedField>();
			UnversionedFields = new List<IUnversionedField>();
			VersionedFields = new List<IVersionedField>();
		}

		public IList<ISharedField> SharedFields { get; private set; }
		public IList<IUnversionedField> UnversionedFields { get; private set; }
		public IList<IVersionedField> VersionedFields { get; private set; }
	}

	public interface ISharedField
	{
		Guid ItemId { get; }
		Guid FieldId { get; }
		string Value { get; }
	}

	public interface IUnversionedField : ISharedField
	{
		string Language { get; }
	}

	public interface IVersionedField : IUnversionedField
	{
		int Version { get; }
	}
}
