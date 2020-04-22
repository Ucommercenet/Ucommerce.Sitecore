using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore;
using Sitecore.Sites;
using Ucommerce.Content;

namespace Ucommerce.Sitecore.Content
{
	public class SitecoreDomainService : IDomainService
	{
		private IEnumerable<Site> Sites
		{
			get { return SiteManager.GetSites().ToList(); }
		}

		/// <summary>
		/// Gets a domain from a specific domain name.
		/// </summary>
		/// <param name="domainName"></param>
		/// <returns></returns>
		public virtual Domain GetDomain(string domainName)
		{
			Site site = Sites.FirstOrDefault(x => x.Name == domainName);

			if (site == null) return null;

			return CreateFromSite(site);
		}

		/// <summary>
		/// Gets the current domain, resolved by SiteCore context.
		/// </summary>
		/// <returns></returns>
		public virtual Domain GetCurrentDomain()
		{
			if (Context.Site == null) return null;

			return GetDomain(Context.Site.Name);
		}

		/// <summary>
		/// Returns a list of domains created from registered sites in SiteCore.
		/// </summary>
		public virtual IEnumerable<Domain> GetDomains()
		{
			return Sites.Select(CreateFromSite).ToList();
		}

		/// <summary>
		/// Maps a Sitecore site to a uCommerce <see cref="Domain"/>.
		/// </summary>
		/// <param name="site">Sitecore site</param>
		/// <returns>Domain</returns>
		private Domain CreateFromSite(Site site)
		{
			var cultureInfo = GetCultureInfoFromRegisteredSite(site);

			var domain = new Domain(site.Name, site.Name, cultureInfo);

			return domain;
		}

		/// <summary>
		/// Returns a sites cultureinfo, defaulting to en-US.
		/// </summary>
		/// <param name="site">Registered site in web.config</param>
		/// <returns>CultureInfo registered on a site</returns>
		/// <remarks> as language specification isn't required, site might not have language configured.</remarks>
		private CultureInfo GetCultureInfoFromRegisteredSite(Site site)
		{
			var languageInfo = string.Empty;

			if (site.Properties.ContainsKey("language"))
				languageInfo = site.Properties["language"];

			if (string.IsNullOrEmpty(languageInfo))
				return new CultureInfo("en-US");

			return new CultureInfo(languageInfo);
		}
	}
}
