using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class RenameConfigDefaultFilesToConfigFiles : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public RenameConfigDefaultFilesToConfigFiles(DirectoryInfo sitecoreDirectory,
            IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;

            Steps.AddRange(new IStep[]
            {
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Configuration")),
                    false,
                    _loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Pipelines")),
                    false,
                    _loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Apps")),
                    false,
                    _loggingService)
            });
        }

        protected override void LogStart()
        {
            _loggingService.Information<RenameConfigDefaultFilesToConfigFiles>("Moving Sitecore configs...");
        }
    }
}
