﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class SitecoreWebconfigMerger : IStep
    {
        private readonly DirectoryInfo _baseDirectory;
        private readonly IInstallerLoggingService _loggingService;

        public SitecoreWebconfigMerger(DirectoryInfo baseDirectory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _baseDirectory = baseDirectory;
        }

        public async Task Run()
        {
            _loggingService.Information<SitecoreWebconfigMerger>("Merging sitecore and ucommerce config files");
            var ucommerceInstallPath = Path.Combine("sitecore modules", "Shell", "ucommerce", "install");
            var mergeConfig = new MergeConfig(Path.Combine(_baseDirectory.FullName, "web.config"),
                new List<Transformation>
                {
                    new Transformation(Path.Combine(_baseDirectory.FullName, ucommerceInstallPath, "CleanConfig.config")),
                    new Transformation(Path.Combine(_baseDirectory.FullName, ucommerceInstallPath, "uCommerce.config")),
                    new Transformation(Path.Combine(_baseDirectory.FullName, ucommerceInstallPath, "uCommerce.IIS7.config"),
                        isIntegrated: true),
                    new Transformation(Path.Combine(_baseDirectory.FullName,
                        ucommerceInstallPath,
                        "uCommerce.dependencies.sitecore.config")),
                    new Transformation(Path.Combine(_baseDirectory.FullName, ucommerceInstallPath, "sitecore.config")),
                    new Transformation(Path.Combine(_baseDirectory.FullName, ucommerceInstallPath, "ClientDependency.config")),
                    new Transformation(Path.Combine(_baseDirectory.FullName,
                        ucommerceInstallPath,
                        "updateAssemblyBinding.config"))
                }
            );

            mergeConfig.Run(new DefaultOutput(), new ConfigurationAttributeCollection());
        }
    }
}
