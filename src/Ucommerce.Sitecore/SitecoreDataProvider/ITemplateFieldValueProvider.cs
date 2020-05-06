using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Tree;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// This interface represents objects capable of providing values for fields of items.
	/// </summary>
	internal interface ITemplateFieldValueProvider
	{
		/// <summary>
		/// If the node type is supported, returns the correct TemplateId for it.
		/// </summary>
		/// <param name="node">The node in question.</param>
		/// <returns>The template id valid for the node.</returns>
		ID GetTemplateId(ITreeNodeContent node);

		/// <summary>
		/// Adds data to the fields for the specific node.
		/// The data returned should match the language specified in the VersionUri
		/// parameter for multilingual fields.
		/// </summary>
		/// <param name="node">The node in question.</param>
		/// <param name="list">The field list to add data to.</param>
		/// <param name="version">The version to add data for. Specifies the language used.</param>
		void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version);

		/// <summary>
		/// Updates data to the ucommerce item specified by the node.
		/// </summary>
		/// <param name="node">The node to save.</param>
		/// <param name="changes">The changes to save.</param>
		/// <returns>True, if the changes were saved. Otherwise false.</returns>
		bool SaveItem(ITreeNodeContent node, ItemChanges changes);

		/// <summary>
		/// Returns true, if this field value provider supports the node.
		/// </summary>
		/// <param name="node">The node to support.</param>
		/// <returns>True, if the node is supported. False, otherwise.</returns>
		bool Supports(ITreeNodeContent node);
	}
}
