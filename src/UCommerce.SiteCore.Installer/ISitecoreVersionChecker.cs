using System;

namespace Ucommerce.Sitecore.Installer
{
    public interface ISitecoreVersionChecker
    {
        bool IsEqualOrGreaterThan(Version version);
        bool IsLowerThan(Version version);
        bool SupportsSpeakApps();
    }
}
