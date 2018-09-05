using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer
{
    public class SitecoreDatabaseAvailabilityService : IDatabaseAvailabilityService
    {
        public bool IsAvailable()
        {
            return true;
        }
    }
}
