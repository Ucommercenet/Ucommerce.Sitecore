using System.Collections.Generic;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer
{
	public class DbInstallerSitecore : DbInstaller
	{
		public DbInstallerSitecore(ConnectionStringLocator locator, IList<Migration> migrations, IInstallerLoggingService loggingService) : base(locator, migrations, loggingService)
		{
			MigrationName = "Sitecore";
			SchemaVersionTable = "uCommerce_SystemVersionSitecore";
		}
	}
}
