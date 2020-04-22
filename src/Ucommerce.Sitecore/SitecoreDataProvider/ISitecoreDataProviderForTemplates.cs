using Sitecore.Collections;
using Sitecore.Data.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	internal interface ISitecoreDataProviderForTemplates : ISitecoreDataProvider
	{
		/// <summary>
		/// Returns a list of ID's of items representing data templates.
		/// </summary>
		/// <returns>A list of template ID's.</returns>
		IDList GetTemplateIds();

		/// <summary>
		/// Returns a list of static templates.
		/// </summary>
		/// <returns>The templates.</returns>
		TemplateCollection GetTemplates();
	}
}