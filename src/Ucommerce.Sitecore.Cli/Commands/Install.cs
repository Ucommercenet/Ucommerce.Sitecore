using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("install")]
    public class Install : ICommand
    {
        [CommandParameter(0, Description = "Connection string for the database.")]
        public string ConnectionString { get; set; }

        [CommandParameter(1, Description = "Path to Sitecore.")]
        public string SitecorePath { get; set; }

        [CommandOption("backupdb", 'b', Description = "Do a backup of the current database.")]
        public bool BackupDatabase { get; set; } = false;

        [CommandOption("upgradedb", 'u', Description = "Upgrade the database.")]
        public bool UpgradeDatabase { get; set; } = false;

        public virtual ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbUpgrade>("Installing...");
            var installStep = new InstallStep(ConnectionString, SitecorePath, BackupDatabase, UpgradeDatabase, logging);
            installStep.Run();

            return default;
        }
    }
}
