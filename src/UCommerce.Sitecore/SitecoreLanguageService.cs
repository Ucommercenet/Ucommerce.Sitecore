using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Web.UI.XamlSharp.Xaml;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.Sitecore
{
    /// <summary>
    /// Extracts installed languages from Sitecore.
    /// </summary>
    public class SitecoreLanguageService : ILanguageService
    {
        private readonly ISitecoreContext _context;

        public SitecoreLanguageService(ISitecoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all configured languages from Sitecore.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method converts from Sitecore languages to uCommerce <see cref="Language"/>list.</remarks>
        public IList<Language> GetAllLanguages()
        {
			ChildList sitecoreLanguageItems = GetInstalledSitecoreLanguageItems();	
			
			IList<Language> languageFromItems = new List<Language>();
			
	        foreach (Item languageItem in sitecoreLanguageItems)
	        {
				string languageDisplayName = TryGetDisplayNameWithFallback(languageItem);

                // Only add the language if one with the same culture code
                // wasn't already added. Have seen Sitecore allow multiple languages
                // with the same culture code sometimes.
                if (languageFromItems.All(x => x.CultureCode != languageItem.Name))
				    languageFromItems.Add(new Language(languageDisplayName, languageItem.Name));
	        }
			
	        return languageFromItems;
        }

	    protected virtual string TryGetDisplayNameWithFallback(Item languageItem)
	    {
		    try
		    {
			    return new CultureInfo(languageItem.Name).DisplayName;
		    }
		    catch (CultureNotFoundException)
		    {
			    //Copying data between different environments may cause a culture not found exception when working with custom installed cultures.
			    return languageItem.Name;
		    }
	    }

	    /// <summary>
		/// Gets all installed languages from sitecore as items. 
		/// </summary>
	    protected virtual ChildList GetInstalledSitecoreLanguageItems()
	    {
			var installedSitecoreLanguages = LanguageManager.GetLanguages(_context.MasterDatabase).Distinct().ToList();

			// LanguageService does not observe sort order
			// of languages from Sitecore. We have to get the
			// parent of the languages and use that to load
			// the children to get the proper sort order.
			var randomLangauge = installedSitecoreLanguages.First();
			Item randomLanguageItem = _context.MasterDatabase.GetItem(randomLangauge.Origin.ItemId);

			// Item.Name is validated by Sitecore as a valid culture
			// so it's safe to new up a CultureInfo and use that
			// for DisplayName.
			return randomLanguageItem.Parent.GetChildren();
	    }
    }
}
