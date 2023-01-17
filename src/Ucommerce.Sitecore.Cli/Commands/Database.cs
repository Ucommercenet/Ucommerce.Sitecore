using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

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
    public class DbUpgrade : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Upgrading database..");

            return default;
        }
    }

    // Child of db command
    [Command("db backup")]
    public class DbBackup : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Backing up database..");

            return default;
        }
    }
}