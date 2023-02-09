using System;
using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Install;
using Ucommerce.Sitecore.Install.Steps;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("install")]
    public class Install : ICommand
    {
        [CommandOption("ConnectionString",
            'c',
            Description = "Connection string for the database.",
            IsRequired = true,
            EnvironmentVariable = "UCOMMERCE_CONNECTION_STRING")]
        // ReSharper disable once MemberCanBeProtected.Global
        public string ConnectionString { get; set; }

        [CommandOption("noDB", 'n', Description = "Install Ucommerce without upgrading the database. Mainly used for debug purposes.")]
        public bool IgnoreDatabase { get; set; } = false;

        [CommandOption("SitecorePath",
            's',
            Description = "Path to Sitecore.",
            IsRequired = true,
            EnvironmentVariable = "UCOMMERCE_SITECORE_PATH")]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string SitecorePath { get; set; }

        public virtual async ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<Install>("Installing...");

            var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var sitecoreDirectory = new DirectoryInfo(SitecorePath);
            var connectionStringLocator = new SitecoreInstallationConnectionStringLocator(ConnectionString);
            var sitecoreDbAvailabilityService = new SitecoreDatabaseAvailabilityService();
            var runtimeVersionChecker = new RuntimeVersionChecker(connectionStringLocator, logging);
            var updateService = new UpdateService(connectionStringLocator, runtimeVersionChecker, sitecoreDbAvailabilityService);
            var versionChecker = new SitecoreVersionCheckerOffline(sitecoreDirectory, logging);

            if (IgnoreDatabase is false)
            {
                var databaseStep = new DatabaseInstallStep(connectionStringLocator,
                    baseDirectory,
                    logging,
                    updateService,
                    runtimeVersionChecker);
                await databaseStep.Run();
            }

            var installStep = new InstallStep(baseDirectory,
                sitecoreDirectory,
                versionChecker,
                logging);
            await installStep.Run();
        }
    }
}
