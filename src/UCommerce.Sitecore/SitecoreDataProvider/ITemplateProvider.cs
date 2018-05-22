using System.Collections.Generic;
using Sitecore.Data;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems;
using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// The main interface for all the uCommerce templates.
	/// </summary>
	public interface ITemplateProvider
	{
		/// <summary>
		/// Returns all the uCommerce templates.
		/// </summary>
		/// <returns>All uCommerce templates.</returns>
		IList<ISitecoreItem> GetTemplates();

		/// <summary>
		/// Retuns a list of field values for a specific tree node and a specific language version.
		/// </summary>
		/// <param name="node">The node to return field values for.</param>
		/// <param name="version">The language version to return field values for.</param>
		/// <returns>A list of field values for the node and language version.</returns>
		FieldList GetFieldList(ITreeNodeContent node, VersionUri version);
	}
}