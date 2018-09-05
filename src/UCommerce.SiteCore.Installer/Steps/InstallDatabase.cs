using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class InstallDatabase : IPostStep
	{
		private readonly DbInstaller _command;

		public InstallDatabase(string migrationsPath)
		{
			var migrationsDirectory = new DirectoryInfo(HostingEnvironment.MapPath(migrationsPath));
			IList<Migration> migrations = new MigrationLoader()
				.GetDatabaseMigrations(migrationsDirectory);

			IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

			InstallationConnectionStringLocator locator = new SitecoreInstallationConnectionStringLocator();

			_command = new DbInstallerCore(locator, migrations, logging);
		}

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			_command.InstallDatabase();
		}
	}
}