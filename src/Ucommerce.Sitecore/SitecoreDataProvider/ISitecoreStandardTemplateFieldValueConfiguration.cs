using System.Collections.Generic;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// The configration of which Sitecore fields to persist for uCommerce Items.
	/// </summary>
	public interface ISitecoreStandardTemplateFieldValueConfiguration
	{
		/// <summary>
		/// A white list of template ids.
		/// </summary>
		IEnumerable<string> WhiteListTemplateIds { get; }

		/// <summary>
		/// A black list of field ids.
		/// </summary>
		IEnumerable<string> BlackListFieldIds { get; }
	}
}
