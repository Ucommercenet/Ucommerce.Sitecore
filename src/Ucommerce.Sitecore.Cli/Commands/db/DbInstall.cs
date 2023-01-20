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
    [Command("db install", Description = "Install a fresh Ucommerce Database")]
    // ReSharper disable once UnusedType.Global
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
}
