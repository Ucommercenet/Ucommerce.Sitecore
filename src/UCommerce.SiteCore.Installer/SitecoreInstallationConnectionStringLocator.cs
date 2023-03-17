using System.Configuration;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer
{
    // Used by the InstallApp Pipeline
    // ReSharper disable once UnusedType.Global
    public class SitecoreInstallationConnectionStringLocator : InstallationConnectionStringLocator
    {
        public override string LocateConnectionString()
        {
            var connectionString = LocateConnectionStringInternal("web");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationException("Unable to locate a connection string in connection strings element called 'uCommerce' or 'web' and connection string configured in CommerceConfiguration does not seem to be valid");
            }

            return connectionString;
        }
    }
}
