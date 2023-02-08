using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class InstallDatabaseUcommerce : IStep
    {
        private readonly DbInstaller _command;

        public InstallDatabaseUcommerce(DirectoryInfo basePath,
            InstallationConnectionStringLocator connectionStringLocator,
            IInstallerLoggingService logging)
        {
            var migrationsDirectory = basePath.CombineDirectory("files", "sitecore modules", "Shell", "Ucommerce", "Install");
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
