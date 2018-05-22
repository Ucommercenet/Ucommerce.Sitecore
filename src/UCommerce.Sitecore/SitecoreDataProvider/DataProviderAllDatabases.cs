using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Globalization;

namespace UCommerce.Sitecore.SitecoreDataProvider
{
	/// <summary>
	/// Abstract base class. Holds all the code common to the uCommerce data providers.
	/// </summary>
	public abstract class DataProviderAllDatabases : DataProvider
	{
		/// <summary>
		/// Overridden default implementation.
		/// </summary>
		/// <remarks>
		/// The default implementation returns two languages "en" and "en-US". This causes
		/// the UI to display two additional versions for each item.
		/// Returning null removes the two "ghost" instances.
		/// </remarks>
		/// <param name="context">The context.</param>
		/// <returns>null</returns>
		public override LanguageCollection GetLanguages(CallContext context)
		{
			return null;
		}

		/// <summary>
		/// Simply return false, to get around a Sitecore bug.
		/// </summary>
		/// <remarks>
		/// The default implementation from Sitecore contains a bug, causing a null reference exception,
		/// when installing packages through the admin page.
		/// This happens because the GetLanguages() method returns null. This is handled fine by the GetItemVersions implementation,
		/// but the RemoveVersions default implementation cannot handle it. :-(
		/// </remarks>
		public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
		{
			return false;
		}

		/// <summary>
		/// Simply return false, to get around a Sitecore bug.
		/// </summary>
		/// <remarks>
		/// The default implementation from Sitecore contains a bug, causing a null reference exception,
		/// when installing packages through the admin page.
		/// This happens because the GetLanguages() method returns null. This is handled fine by the GetItemVersions implementation,
		/// but the RemoveVersions default implementation cannot handle it. :-(
		/// </remarks>
		public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, CallContext context)
		{
			return false;
		}
	}
}
