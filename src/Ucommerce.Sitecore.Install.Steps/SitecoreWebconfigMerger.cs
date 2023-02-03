using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;
using Transformation = Ucommerce.Sitecore.Installer.Steps.Transformation;

namespace Ucommerce.Sitecore.Install.Steps
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
            var ucommerceInstallDirectory = _sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "ucommerce", "install");
            var mergeConfig = new MergeConfig(_sitecoreDirectory.CombineFile("web.config"),
                new List<Transformation>
                {
                    new Transformation(ucommerceInstallDirectory.CombineFile("CleanConfig.config")),
                    new Transformation(ucommerceInstallDirectory.CombineFile("uCommerce.config")),
                    new Transformation(ucommerceInstallDirectory.CombineFile("uCommerce.IIS7.config"), true),
                    new Transformation(ucommerceInstallDirectory.CombineFile("uCommerce.dependencies.sitecore.config")),
                    new Transformation(ucommerceInstallDirectory.CombineFile("sitecore.config")),
                    new Transformation(ucommerceInstallDirectory.CombineFile("ClientDependency.config")),
                    new Transformation(ucommerceInstallDirectory.CombineFile("updateAssemblyBinding.config")),
                },
                _loggingService
            );

            await mergeConfig.Run();
        }
    }
}
