using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class SitecoreWebconfigMerger : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly DirectoryInfo _sitecoreDirectory;

        public SitecoreWebconfigMerger(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _sitecoreDirectory = sitecoreDirectory;
        }

        public async Task Run()
        {
            _loggingService.Information<SitecoreWebconfigMerger>("Merging Sitecore and Ucommerce config files...");
            var ucommerceInstallPath = _sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "ucommerce", "install");
            var mergeConfig = new MergeConfig(_sitecoreDirectory.CombineFile("web.config"),
                new List<Transformation>
                {
                    new Transformation(ucommerceInstallPath.CombineFile("CleanConfig.config")),
                    new Transformation(ucommerceInstallPath.CombineFile("uCommerce.config")),
                    new Transformation(ucommerceInstallPath.CombineFile("uCommerce.IIS7.config"), true),
                    new Transformation(ucommerceInstallPath.CombineFile("uCommerce.dependencies.sitecore.config")),
                    new Transformation(ucommerceInstallPath.CombineFile("sitecore.config")),
                    new Transformation(ucommerceInstallPath.CombineFile("ClientDependency.config")),
                    new Transformation(ucommerceInstallPath.CombineFile("updateAssemblyBinding.config")),
                },
                _loggingService
            );

            await mergeConfig.Run();
        }
    }
}
