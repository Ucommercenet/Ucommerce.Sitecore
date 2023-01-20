using System;
using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Ucommerce.Sitecore.Cli.Logging;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Cli.Commands.db
{
    [Command("db backup")]
    // ReSharper disable once UnusedType.Global
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
