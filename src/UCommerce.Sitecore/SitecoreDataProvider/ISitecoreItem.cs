using System;
using System.Collections.Generic;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// This interface represents what an item needs to provide, in order to be a Sitecore item.
	/// </summary>
	public interface ISitecoreItem
	{
		/// <summary>
		/// The ID of the item.
		/// </summary>
		ID Id { get; }

		/// <summary>
		/// The ItemDefinition of the Item.
		/// </summary>
		ItemDefinition ItemDefinition { get; }

		/// <summary>
		/// Returns a list of field values for the specific version.
		/// </summary>
		/// <param name="version">The version to return fields for.</param>
		/// <returns>Field values for the specific version.</returns>
		FieldList GetFieldList(VersionUri version);

		/// <summary>
		/// Updates the underlying ucommerce item.
		/// </summary>
		/// <param name="changes">The changes to the item.</param>
		/// <returns>True, if the changes were saved. Otherwise false.</returns>
		bool SaveItem(ItemChanges changes);

		/// <summary>
		/// The ID of the parent Item.
		/// </summary>
		ID ParentId { get; set; }

		/// <summary>
		/// The list of ID's of the child items.
		/// </summary>
		/// <returns>List of children's ID's.</returns>
		IDList ChildIds();

		/// <summary>
		/// Check if this item has children.
		/// </summary>
		/// <returns>true, if the item has children.</returns>
		bool HasChildren();

		/// <summary>
		/// True, if this Item represents a Data Template.
		/// </summary>
		/// <returns>True, if item is a template.</returns>
		bool IsTemplate();

		/// <summary>
		/// The Child items.
		/// </summary>
		IEnumerable<ISitecoreItem> Children { get; }

		/// <summary>
		/// Sets the revision guid for this item.
		/// </summary>
		/// <param name="revision">The revision guid.</param>
		void SetRevision(Guid revision);

	}
}
