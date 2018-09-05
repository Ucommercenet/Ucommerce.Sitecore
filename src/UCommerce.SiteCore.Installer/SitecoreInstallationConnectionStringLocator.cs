using System.Configuration;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer
{
    public class SitecoreInstallationConnectionStringLocator : InstallationConnectionStringLocator
    {
        public override string LocateConnectionString()
        {
            var connectionString = LocateConnectionStringInternal("web");
            if (string.IsNullOrEmpty(connectionString))
                throw new ConfigurationException("Unable to locate a connection string in connection strings element called 'uCommerce' or 'web' and connection string configured in CommerceConfiguration does not seem to be valid");

            return connectionString;
        }
    }
}
