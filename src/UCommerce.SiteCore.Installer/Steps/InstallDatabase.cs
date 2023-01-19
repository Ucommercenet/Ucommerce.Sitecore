using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class InstallDatabase : IStep
    {
        private readonly DbInstaller _command;

        public InstallDatabase(DirectoryInfo packageBasePath, IInstallerLoggingService logging, InstallationConnectionStringLocator connectionStringLocator)
        {
            var migrationsDirectory =
                new DirectoryInfo(Path.Combine(packageBasePath.FullName, "package", "files", "sitecore modules", "Shell", "Ucommerce", "Install"));
            var migrations = new MigrationLoader().GetDatabaseMigrations(migrationsDirectory);

            _command = new DbInstallerCore(connectionStringLocator, migrations, logging);
        }

        public async Task Run()
        {
            _command.InstallDatabase();
        }
    }
}
