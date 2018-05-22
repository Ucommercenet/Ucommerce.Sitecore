using System;
using Sitecore.Data;
using UCommerce.Sitecore.SitecoreDataProvider;

namespace UCommerce.Sitecore
{
    /// <summary>
    /// Sitecore Context implementation.
    /// </summary>
    public interface ISitecoreContext
    {
        /// <summary>
        /// Gets the Sitecore Master database.
        /// </summary>
        Database MasterDatabase { get; }

		/// <summary>
		/// Gets the database used when picking content.
		/// </summary>
		Database DatabaseForContent { get; }

		/// <summary>
		/// Gets the main uCommerce data provider.
		/// </summary>
		DataProviderMasterDatabase DataProviderMaster { get; }

		/// <summary>
		/// Gets the BackendDomainName for the sitecore backend.
		/// </summary>
		string BackendDomainName { get; }

		bool ShouldPullTemplatesFromSitecore { get; }
    }
}