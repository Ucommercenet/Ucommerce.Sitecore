using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("upgrade")]
    public class Upgrade : Install
    {
        [CommandOption("backupdb", 'b', Description = "Do a backup of the current database.")]
        public bool BackupDatabase { get; set; } = false;

        [CommandOption("upgradedb", 'u', Description = "Upgrade the database.")]
        public bool UpgradeDatabase { get; set; } = false;

        public override ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Upgrading..");
            if (BackupDatabase) console.Output.WriteLine("Backing up database..");
            if (UpgradeDatabase) console.Output.WriteLine("Upgrading database..");

            return default;
        }
    }
}