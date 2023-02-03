using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class RenameConfigDefaultFilesToConfigFiles : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public RenameConfigDefaultFilesToConfigFiles(DirectoryInfo sitecoreDirectory,
            IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var ucommerceDirectory = sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "uCommerce");
            Steps.AddRange(new IStep[]
            {
                new RenameConfigDefaultFilesToConfigFilesStep(ucommerceDirectory.CombineDirectory("Configuration"),
                    false,
                    _loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(ucommerceDirectory.CombineDirectory("Pipelines"),
                    false,
                    _loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(ucommerceDirectory.CombineDirectory("Apps"),
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
