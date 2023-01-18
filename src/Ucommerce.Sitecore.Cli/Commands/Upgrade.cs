using System.Threading.Tasks;
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
}