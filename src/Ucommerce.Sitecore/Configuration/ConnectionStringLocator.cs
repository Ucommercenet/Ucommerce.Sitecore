using System.Configuration;

namespace Ucommerce.Sitecore.Configuration
{
    public class ConnectionStringLocator : Ucommerce.Infrastructure.Configuration.ConnectionStringLocator
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
