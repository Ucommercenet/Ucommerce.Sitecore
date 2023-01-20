using System;
using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Cli.Commands.db
{
    [Command("db upgrade", Description = "Upgrade the Ucommerce Database")]
    // ReSharper disable once UnusedType.Global
    public class DbUpgrade : Db, ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbUpgrade>("Upgrading database...");

            var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var connectionStringLocator = new SitecoreInstallationConnectionStringLocator(ConnectionString);
            var runtimeVersionChecker = new RuntimeVersionChecker(connectionStringLocator, logging);
            var sitecoreDatabaseAvailabilityService = new SitecoreDatabaseAvailabilityService();
            var updateService = new UpdateService(connectionStringLocator, runtimeVersionChecker, sitecoreDatabaseAvailabilityService);

            var databaseStep = new DatabaseUpgradeStep(connectionStringLocator,
                baseDirectory,
                logging,
                updateService,
                runtimeVersionChecker);

            await databaseStep.Run();
        }
    }
}
