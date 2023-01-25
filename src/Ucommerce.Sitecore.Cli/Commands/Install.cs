using System;
using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer;
using Ucommerce.Sitecore.Installer.Steps;

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
            var connectionStringLocater = new SitecoreInstallationConnectionStringLocator(ConnectionString);
            var versionChecker = new SitecoreVersionCheckerOffline(sitecoreDirectory, logging);
            var installStep = new InstallStep(baseDirectory, sitecoreDirectory, versionChecker, connectionStringLocater, logging);

            await installStep.Run();
        }
    }
}
