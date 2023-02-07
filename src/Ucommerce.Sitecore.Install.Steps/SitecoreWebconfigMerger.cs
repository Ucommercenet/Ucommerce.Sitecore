using System.Collections.Generic;
using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate steps that merges Ucommerce config files into Sitecores web.config
    /// </summary>
    public class SitecoreWebconfigMerger : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public SitecoreWebconfigMerger(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var ucommerceInstallDirectory = sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "ucommerce", "install");
            var mergeConfig = new MergeConfig(sitecoreDirectory.CombineFile("web.config"),
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
            Steps.Add(mergeConfig);
        }

        protected override void LogStart()
        {
            _loggingService.Information<SitecoreWebconfigMerger>("Merging Sitecore and Ucommerce config files...");
        }
    }
}
