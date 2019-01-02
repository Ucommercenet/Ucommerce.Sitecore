using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class InstallDatabaseSitecore : IInstallationStep
    {
        private readonly DbInstaller _command;

        public InstallDatabaseSitecore(string migrationsPath)
        {
            var migrationsDirectory = new DirectoryInfo(HostingEnvironment.MapPath(migrationsPath));
            IList<Migration> migrations = new MigrationLoader()
                .GetDatabaseMigrations(migrationsDirectory);

            IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

            InstallationConnectionStringLocator locator = new SitecoreInstallationConnectionStringLocator();

            _command = new DbInstallerSitecore(locator, migrations, logging);
        }
        public void Execute()
        {
            _command.InstallDatabase();
        }
    }
}