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
    
    [Command("db help")]
    public class DbHelp : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Commands for database:");
            console.Output.WriteLine("install     installs Ucommerce database");
            console.Output.WriteLine("upgrade     upgrades Ucommerce database");
            console.Output.WriteLine("backup     backs up Ucommerce database");
            console.Output.WriteLine();
            console.Output.WriteLine("Flags for database:");
            console.Output.WriteLine("--connection-string     connection string for your database, is required");
            return default;
        }
    }
    
    [Command("db install help")]
    public class DbInstallHelp : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Flags for database installation:");
            console.Output.WriteLine("--connection-string     connection string for your database, is required");
            return default;
        }
    }
    
    [Command("db upgrade help")]
    public class DbUpgradeHelp : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Flags for database installation:");
            console.Output.WriteLine("--connection-string     connection string for your database, is required");
            return default;
        }
    }
    
    [Command("db backup help")]
    public class DbBackupHelp : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine("Flags for database installation:");
            console.Output.WriteLine("--connection-string     connection string for your database, is required");
            return default;
        }
    }
}