using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.Presentation.Views.Marketing;

namespace UCommerce.Sitecore.SitecoreDataProvider
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
