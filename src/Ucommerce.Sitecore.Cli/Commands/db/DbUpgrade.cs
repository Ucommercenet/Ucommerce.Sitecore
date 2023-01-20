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
    [Command("db upgrade", Description = "Upgrade the Ucommerce Database")]
    // ReSharper disable once UnusedType.Global
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
}
