using System;
using Sitecore.Configuration;

namespace Ucommerce.Sitecore.Installer
{
    public class SitecoreVersionCheckerOnline : ISitecoreVersionChecker
    {
        public bool IsLowerThan(Version version)
        {
            Version actualVersion;
            if (Version.TryParse(About.Version, out actualVersion))
            {
                return actualVersion < version;
            }

            return false;
        }

        public bool IsEqualOrGreaterThan(Version version)
        {
            if (Version.TryParse(About.Version, out var actualVersion))
            {
                return actualVersion >= version;
            }

            return false;
        }

        public bool SupportsSpeakApps()
        {
            var majorVersionNumberAsString = About.Version.Substring(0, About.Version.IndexOf('.'));
            var majorVersionNumber = int.Parse(majorVersionNumberAsString);

            return majorVersionNumber >= 8;
        }
    }
}
