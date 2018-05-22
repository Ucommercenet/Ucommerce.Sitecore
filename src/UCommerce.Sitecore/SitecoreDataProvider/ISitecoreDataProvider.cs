using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// This interface is for our internal sitecore data providers.
	/// It represents what is needed for our main data provider to
	/// provide sitecore with Item information.
	/// </summary>
	internal interface ISitecoreDataProvider
	{
		/// <summary>
		/// Returns the ID of the item, the data is hooked into.
		/// </summary>
		/// <returns>The ID of the sitecore entry point item.</returns>
		ID GetEntryIdInSitecoreTree();

		/// <summary>
		/// Sets the entry point in the Sitecore tree.
		/// </summary>
		/// <param name="entryPoint">The ID of the item to return children for.</param>
		void SetEntryIdInSitecoreTree(ID entryPoint);

		/// <summary>
		/// Returns the first list of ID's to be placed under the entry point.
		/// </summary>
		/// <returns>List of ID's.</returns>
		IDList GetFirstLevelIds();

		/// <summary>
		/// Returns true, if the ID is an item this data provider handles.
		/// </summary>
		/// <param name="id">The ID of the item.</param>
		/// <returns>True, if it is one of our items.</returns>
		bool IsOneOfOurSitecoreItems(ID id);

		/// <summary>
		/// Returns the ItemDefinition for the specified ID.
		/// </summary>
		/// <param name="id">The ID to return an ItemDefinition for.</param>
		/// <returns>The ItemDefinition for the ID.</returns>
		ItemDefinition GetItemDefinition(ID id);

		/// <summary>
		/// Returns a list of child ID's for the specified ID.
		/// </summary>
		/// <param name="id">The ID of the item.</param>
		/// <returns>A list of child ID's.</returns>
		IDList GetChildIds(ID id);

		/// <summary>
		/// Returns true, if the item has children.
		/// </summary>
		/// <param name="id">The id of the item.</param>
		/// <returns>true, if the item has children.</returns>
		bool HasChildren(ID id);

		/// <summary>
		/// Returns the parent ID of the item for the specified ID.
		/// </summary>
		/// <param name="id">The ID of the item in question.</param>
		/// <returns>The parent ID of the item in question.</returns>
		ID GetParentId(ID id);

		/// <summary>
		/// Returns a list of field values for a specific item.
		/// </summary>
		/// <param name="id">The ID of the item in question.</param>
		/// <param name="version">The version to return data for.</param>
		/// <returns>A list of field values for a specific item in a specific version.</returns>
		FieldList GetFieldList(ID id, VersionUri version);

		/// <summary>
		/// returns a list of versions (languages) for this data providers items.
		/// </summary>
		/// <returns></returns>
		VersionUriList GetItemVersions();

		/// <summary>
		/// Support for saving changes to an item.
		/// </summary>
		/// <param name="item">The item that has changed.</param>
		/// <param name="changes">The changes to the item.</param>
		/// <returns>True, if the changes was saved, otherwise false.</returns>
		bool SaveItem(ItemDefinition item, ItemChanges changes);

		/// <summary>
		/// Clears the internal data of the data provider.
		/// </summary>
		void Clear();

		void ProductSaved(Product product);

		void ProductDeleted(Product product);

		void VariantDeleted(Product variant);

		void CategorySaved(Category category);

		void CatalogSaved(ProductCatalog catalog);

		void StoreSaved(ProductCatalogGroup store);

		void ProductDefinitionSaved(ProductDefinition definition);

		void DefinitionSaved(IDefinition definition);

		void DefinitionFieldSaved(IDefinitionField field);

		void LanguageSaved(Language language);

		void DataTypeSaved(DataType dataType);

	    void PermissionsChanged();
	}
}