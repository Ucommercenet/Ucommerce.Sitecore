using System.Collections.Generic;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
	public class DbInstallerSitecore : DbInstaller
	{
		public DbInstallerSitecore(InstallationConnectionStringLocator locator, IList<Migration> migrations, IInstallerLoggingService loggingService) : base(locator, migrations, loggingService)
		{
			MigrationName = "Sitecore";
			SchemaVersionTable = "uCommerce_SystemVersionSitecore";
		}
	}
}
