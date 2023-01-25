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
         
            return default;
        }
    }
}