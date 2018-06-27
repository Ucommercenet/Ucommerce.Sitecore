using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitecore.Configuration
{
    public class ConnectionStringLocator : UCommerce.Infrastructure.Configuration.ConnectionStringLocator
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
