using System.Collections.Generic;
using System.Linq;
using System;
using System.Web;
using Sitecore.Configuration;
using UCommerce.Infrastructure.Logging;

namespace UCommerce.Sitecore.UI.Resources
{
    /// <summary>
    /// Resolves the version of the current Sitecore environment.
    /// </summary>
    public class SitecoreVersionResolver : ISitecoreVersionResolver
    {
	    private readonly ILoggingService _loggingService;

	    public SitecoreVersionResolver(ILoggingService loggingService)
	    {
		    _loggingService = loggingService;
	    }

	    /// <summary>
        /// Returns whether or not the current Sitecore version is 7.x.
        /// </summary>
        public bool IsSitecore7
        {
            get { return About.Version.StartsWith("7."); }
        }

        /// <summary>
        /// Returns whether or not the current Sitecore version is 8.x.
        /// </summary>
        public bool IsSitecore8
        {
            get { return About.Version.StartsWith("8."); }
        }

	    public bool IsEqualOrGreaterThan(Version version)
	    {
		    Version actualVersion;
		    if (Version.TryParse(About.Version, out actualVersion))
		    {
				return actualVersion >= version;
		    }

			_loggingService.Log<SitecoreVersionResolver>(string.Format("Couldn't determine the version of Sitecore based on {0}", About.Version));

		    return false;
	    }

	    /// <summary>
        /// Returns whether or not the current shell is loaded in a SPEAK context.
        /// </summary>
        /// <remarks>
        /// This should only be used to determine if the shell is loaded as a SPEAK application, since we have no way
        /// of knowing if a specific page loaded in a shell is contained in a SPEAK shell.
        /// </remarks>
        public bool IsSpeakApplication
        {
            get
            {
                var qs = HttpContext.Current.Request.QueryString["IsSpeak"];
                bool isSpeak;

                if (!bool.TryParse(qs, out isSpeak))
                    isSpeak = false;

                return isSpeak;
            }
        }
    }
}