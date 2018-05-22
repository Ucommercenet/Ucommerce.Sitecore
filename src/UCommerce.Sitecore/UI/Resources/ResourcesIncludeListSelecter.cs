using System;
using System.Web.UI;
using UCommerce.Presentation.UI.Resources;
using UCommerce.Sitecore.UI.Resources.Version7;
using UCommerce.Sitecore.UI.Resources.Version8;

namespace UCommerce.Sitecore.UI.Resources
{
    /// <summary>
    /// Selects which client resources to load for the current page.
    /// </summary>
	public class ResourcesIncludeListSelecter : IResourcesIncludeList
	{
        private readonly ISitecoreVersionResolver _sitecoreVersionResolver;

        public ResourcesIncludeListSelecter(ISitecoreVersionResolver sitecoreVersionResolver)
        {
            _sitecoreVersionResolver = sitecoreVersionResolver;
        }

        public Control GetControl()
		{
            // TODO: can be removed?

			//if (IsSpeakApplication()) return new ResourcesIncludeListSitecore8Speak().GetControl();
			if (_sitecoreVersionResolver.IsEqualOrGreaterThan(new Version(8, 2))) return new Version82.ResourcesIncludeListSitecore().GetControl();
			if (_sitecoreVersionResolver.IsSitecore8) return new Version8.ResourcesIncludeListSitecore().GetControl();
			if (_sitecoreVersionResolver.IsSitecore7) return new Version7.ResourcesIncludeListSitecore().GetControl();

			// default.
			return new Version7.ResourcesIncludeListSitecore().GetControl();
		}
	}
}
