using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;

namespace Ucommerce.Sitecore.Cli.Commands.db
{
    [Command("db backup")]
    // ReSharper disable once UnusedType.Global
    public class DbBackup : Db, ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbBackup>("Backing up database...");
            logging.Information<DbBackup>("This command has not yet been implemented");

            return default;
        }
    }
}
