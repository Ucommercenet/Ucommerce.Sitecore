using System.Collections.Generic;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// The template builder is responsible for dynamically generating Sitecore templates
	/// used for displaying specific uCommerce data.
	/// 
	/// The builder is also responsible for providing data for the fields of the template.
	/// </summary>
	internal interface ITemplateBuilder : ITemplateFieldValueProvider
	{
		/// <summary>
		/// Build the templates used for the tree nodes this builder is supports.
		/// </summary>
		/// <returns>A list of templates.</returns>
		IEnumerable<ISitecoreItem> BuildTemplates();
	}
}