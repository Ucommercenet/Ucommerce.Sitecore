using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install
{
    public class SitecoreDatabaseAvailabilityService : IDatabaseAvailabilityService
    {
        public bool IsAvailable()
        {
            return true;
        }
    }
}
