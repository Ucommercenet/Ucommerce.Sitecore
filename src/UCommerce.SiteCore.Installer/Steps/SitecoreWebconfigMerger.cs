using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

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
            var ucommerceInstallPath = Path.Combine("sitecore modules", "Shell", "ucommerce", "install");
            var mergeConfig = new MergeConfig(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, "web.config")),
                new List<Transformation>
                {
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, ucommerceInstallPath, "CleanConfig.config"))),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, ucommerceInstallPath, "uCommerce.config"))),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, ucommerceInstallPath, "uCommerce.IIS7.config")),
                        isIntegrated: true),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName,
                        ucommerceInstallPath,
                        "uCommerce.dependencies.sitecore.config"))),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, ucommerceInstallPath, "sitecore.config"))),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName, ucommerceInstallPath, "ClientDependency.config"))),
                    new Transformation(new FileInfo(Path.Combine(_sitecoreDirectory.FullName,
                        ucommerceInstallPath,
                        "updateAssemblyBinding.config")))
                },
                _loggingService
            );

            await mergeConfig.Run();
        }
    }
}
