using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Cli.Commands
{
    [Command("db")]
    public class Database : ICommand
    {
        [CommandParameter(0, Description = "Connection string for the database.")]
        public string ConnectionString { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            return default;
        }
    }

    // Child of db command
    [Command("db upgrade", Description = "")]
    public class DbUpgrade : Database, ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbUpgrade>("Upgrading database..");

            var DatabaseStep = new DatabaseStep(ConnectionString, upgradeDb: true);
            DatabaseStep.Run();

            return default;
        }
    }

    // Child of db command
    [Command("db backup")]
    public class DbBackup : Database, ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbUpgrade>("Backing up database..");

            var DatabaseStep = new DatabaseStep(ConnectionString, true);
            DatabaseStep.Run();

            return default;
        }
    }
}