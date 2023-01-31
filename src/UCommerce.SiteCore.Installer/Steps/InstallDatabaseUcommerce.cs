using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class InstallDatabaseUcommerce : IStep
    {
        private readonly DbInstaller _command;

        public InstallDatabaseUcommerce(DirectoryInfo packageBasePath, InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService logging)
        {
            var migrationsDirectory =
                new DirectoryInfo(Path.Combine(packageBasePath.FullName, "package", "files", "sitecore modules", "Shell", "Ucommerce", "Install"));
            var migrations = new MigrationLoader().GetDatabaseMigrations(migrationsDirectory);

            _command = new DbInstallerCore(connectionStringLocator, migrations, logging);
        }

        public Task Run()
        {
            _command.InstallDatabase();
            return Task.CompletedTask;
        }
    }
}
