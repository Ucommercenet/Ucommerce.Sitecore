using System;
using System.Web.UI;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources
{
    /// <summary>
    /// Selects which client resources to load for the current shell.
    /// </summary>
	public class ResourcesIncludeListShellSelecter : IResourcesIncludeList
	{
        private readonly ISitecoreVersionResolver _sitecoreVersionResolver;

        public ResourcesIncludeListShellSelecter(ISitecoreVersionResolver sitecoreVersionResolver)
        {
            _sitecoreVersionResolver = sitecoreVersionResolver;
        }

        public Control GetControl()
		{
	        if (_sitecoreVersionResolver.IsEqualOrGreaterThan(new Version(8, 2)) && _sitecoreVersionResolver.IsSpeakApplication)
	        {
				return new Version82.ResourcesIncludeListSitecoreSpeakShell().GetControl();
	        }
			if (_sitecoreVersionResolver.IsEqualOrGreaterThan(new Version(8, 2))) return new Version82.ResourcesIncludeListSitecoreShell().GetControl();
			if (_sitecoreVersionResolver.IsSpeakApplication) return new Version8.ResourcesIncludeListSitecoreSpeakShell().GetControl();
			if (_sitecoreVersionResolver.IsSitecore8) return new Version8.ResourcesIncludeListSitecoreShell().GetControl();
			if (_sitecoreVersionResolver.IsSitecore7) return new Version7.ResourcesIncludeListSitecoreShell().GetControl();

			// default.
			return new Version7.ResourcesIncludeListSitecoreShell().GetControl();
		}
	}
}
