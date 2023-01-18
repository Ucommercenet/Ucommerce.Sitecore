using System.Configuration;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer
{
    public class SitecoreInstallationConnectionStringLocator : InstallationConnectionStringLocator
    {
        private readonly string _connectionString;

        public SitecoreInstallationConnectionStringLocator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override string LocateConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ConfigurationErrorsException(
                    "No connection string was given");

            return _connectionString;
        }
    }
}