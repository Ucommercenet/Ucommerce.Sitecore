using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("upgrade")]
    public class Upgrade : Install
    {
        public override ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Upgrading..");
            if (BackupDatabase) console.Output.WriteLine("Backing up database..");
            if (UpgradeDatabase) console.Output.WriteLine("Upgrading database..");

            return default;
        }
    }
    
    [Command("upgrade help")]
    public class UpgradeHelp : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Flags for upgrading:");
            console.Output.WriteLine("--db-upgrade     upgrades the database");
            console.Output.WriteLine("--db-backup     backs up the database");
            console.Output.WriteLine("--connection-string     connection string for your database, is required");
            console.Output.WriteLine("--sitecore-path     path for sitecore installation, is required");
            return default;
        }
    }
}