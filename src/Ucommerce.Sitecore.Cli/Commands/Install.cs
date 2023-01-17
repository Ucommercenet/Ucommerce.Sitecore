using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("install")]
    public class Install : ICommand
    {
        [CommandParameter(0, Description = "Connection string for the database.")]
        public string ConnectionString { get; set; }

        [CommandParameter(1, Description = "Path to sitecore.")]
        public string SitecorePath { get; set; }

        [CommandOption("backupdb", 'b', Description = "Do a backup of the current database.")]
        public bool BackupDatabase { get; set; } = false;

        [CommandOption("upgradedb", 'u', Description = "Upgrade the database.")]
        public bool UpgradeDatabase { get; set; } = false;

        public virtual ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Installing..");
            if (BackupDatabase) console.Output.WriteLine("Backing up database..");
            if (UpgradeDatabase) console.Output.WriteLine("Upgrading database..");

            return default;
        }
    }
}