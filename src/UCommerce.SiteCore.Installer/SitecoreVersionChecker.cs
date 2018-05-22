using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;

namespace UCommerce.Sitecore.Installer
{
    public class SitecoreVersionChecker
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
            Version actualVersion;
            if (Version.TryParse(About.Version, out actualVersion))
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
