using Sitecore.Configuration;
using Sitecore.Data;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider;

namespace Ucommerce.Sitecore
{
    /// <summary>
    /// Sitecore Context implementation.
    /// </summary>
    public class SitecoreContext : ISitecoreContext
    {
	    private string NameOfContentDatabase { get; set; }

	    public SitecoreContext(string backEndDomainName, string nameOfContentDatabase, bool shouldPullTemplatesFromSitecore)
	    {
		    BackendDomainName = backEndDomainName;
		    NameOfContentDatabase = nameOfContentDatabase;
		    ShouldPullTemplatesFromSitecore = shouldPullTemplatesFromSitecore;
	    }

		public virtual Database MasterDatabase
        {
            get { return Factory.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName, false); }
        }

	    public virtual Database DatabaseForContent
	    {
		    get { return Factory.GetDatabase(NameOfContentDatabase); }
	    }

	    public DataProviderMasterDatabase DataProviderMaster
	    {
		    get
		    {
			    return MasterDatabase != null ? MasterDatabase.GetUcommerceDataProvider() : null;
		    }
	    }

	    public string BackendDomainName { get; private set; }

	    public bool ShouldPullTemplatesFromSitecore { get; private set; }
    }
}