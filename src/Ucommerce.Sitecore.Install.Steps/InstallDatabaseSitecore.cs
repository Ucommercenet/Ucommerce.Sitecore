using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step to set up a database for Sitecore
    /// </summary>
    public class InstallDatabaseSitecore : IStep
    {
        private readonly DbInstaller _command;

        public InstallDatabaseSitecore(DirectoryInfo basePath, InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService logging)
        {
            var migrationsDirectory =
                basePath.CombineDirectory("package", "files", "sitecore modules", "Shell", "Ucommerce", "Install");
            var migrations = new MigrationLoader().GetDatabaseMigrations(migrationsDirectory);

            _command = new DbInstallerSitecore(connectionStringLocator, migrations, logging);
        }

        public Task Run()
        {
            _command.InstallDatabase();
            return Task.CompletedTask;
        }
    }
}
