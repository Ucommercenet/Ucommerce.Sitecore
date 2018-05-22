using System;

namespace UCommerce.Sitecore.UI.Resources
{
    /// <summary>
    /// Resolves the version of the current Sitecore environment.
    /// </summary>
    public interface ISitecoreVersionResolver
    {
        /// <summary>
        /// Returns whether or not the current Sitecore version is 7.x.
        /// </summary>
        bool IsSitecore7 { get; }

        /// <summary>
        /// Returns whether or not the current Sitecore version is 8.x.
        /// </summary>
        bool IsSitecore8 { get; }

	    bool IsEqualOrGreaterThan(Version version);

        /// <summary>
        /// Returns whether or not the current shell is loaded in a SPEAK context.
        /// </summary>
        /// <remarks>
        /// This should only be used to determine if the shell is loaded as a SPEAK application, since we have no way
        /// of knowing if a specific page loaded in a shell is contained in a SPEAK shell.
        /// </remarks>
        bool IsSpeakApplication { get; }
    }
}