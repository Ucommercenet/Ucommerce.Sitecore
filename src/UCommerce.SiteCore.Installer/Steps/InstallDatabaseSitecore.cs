using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class InstallDatabaseSitecore : IPostStep
	{
		private readonly DbInstaller _command;

		public InstallDatabaseSitecore(string migrationsPath)
		{
			var migrationsDirectory = new DirectoryInfo(HostingEnvironment.MapPath(migrationsPath));
			IList<Migration> migrations = new MigrationLoader()
				.GetDatabaseMigrations(migrationsDirectory);

			IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

			var locator = new SitecoreInstallationConnectionStringLocator();

			_command = new DbInstallerSitecore(locator, migrations, logging);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.InstallDatabase();
		}
	}
}
