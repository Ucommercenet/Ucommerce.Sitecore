using System;
using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Cli.Commands
{
    /// <summary>
    /// Base class for db commands exposing shared Options
    /// </summary>
    public abstract class Db
    {
        [CommandOption("ConnectionString",
            'c',
            Description = "Connection string for the database.",
            IsRequired = true,
            EnvironmentVariable = "UCOMMERCE_CONNECTION_STRING")]
        public string ConnectionString { get; set; }
    }

    [Command("db upgrade", Description = "Upgrade the Ucommerce Database")]
    public class DbUpgrade : Db, ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbUpgrade>("Upgrading database...");

            var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var databaseStep = new DatabaseStep(ConnectionString, baseDirectory, logging, upgradeDb: true);
            await databaseStep.Run();
        }
    }

    [Command("db install", Description = "Install a fresh Ucommerce Database")]
    public class DbInstall : Db, ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbInstall>("Installing database...");

            var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var databaseStep = new DatabaseStep(ConnectionString, baseDirectory, logging, upgradeDb: true);
            await databaseStep.Run();
        }
    }

    [Command("db backup")]
    public class DbBackup : Db, ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var logging = new CliLogger(console);
            logging.Information<DbBackup>("Backing up database...");

            var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var databaseStep = new DatabaseStep(ConnectionString, baseDirectory, logging, backupDb: true);
            await databaseStep.Run();
        }
    }
}
