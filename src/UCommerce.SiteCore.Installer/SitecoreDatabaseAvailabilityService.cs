using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer
{
    public class SitecoreDatabaseAvailabilityService : IDatabaseAvailabilityService
    {
        public bool IsAvailable()
        {
            return true;
        }
    }
}
