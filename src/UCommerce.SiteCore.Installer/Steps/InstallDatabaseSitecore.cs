using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class InstallDatabaseSitecore : IStep
    {
        private readonly DbInstaller _command;

        public InstallDatabaseSitecore(DirectoryInfo packageBasePath, InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService logging)
        {
            var migrationsDirectory =
                new DirectoryInfo(Path.Combine(packageBasePath.FullName, "package", "files", "sitecore modules", "Shell", "Ucommerce", "Install"));
            var migrations = new MigrationLoader().GetDatabaseMigrations(migrationsDirectory);

            _command = new DbInstallerSitecore(connectionStringLocator, migrations, logging);
        }

        public async Task Run()
        {
            _command.InstallDatabase();
        }
    }
}
