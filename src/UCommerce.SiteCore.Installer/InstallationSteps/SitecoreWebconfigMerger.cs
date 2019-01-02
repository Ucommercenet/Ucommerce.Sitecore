using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MergeConfig : MergeConfigKeepingConnectionStringValue, IInstallationStep
    {
        private readonly FileInfo _toBeTransformed;
        public IList<Transformation> Transformations { get; set; }

        public MergeConfig(string configurationVirtualPath, IList<Transformation> transformations)
        {
            InitializeTargetDocumentPath(configurationVirtualPath);
            Transformations = transformations;
            _toBeTransformed = new FileInfo(HostingEnvironment.MapPath(configurationVirtualPath));
        }

        public void Execute()
        {
            ReadConnectionStringAttribute();

            using (var transformer = new ConfigurationTransformer(_toBeTransformed))
            {
                foreach (var transformation in Transformations)
                {
                    transformer.Transform(
                        new FileInfo(HostingEnvironment.MapPath(transformation.VirtualPath)),
                        transformation.OnlyIfIisIntegrated,
                        ex => new SitecoreInstallerLoggingService().Log<int>(ex));
                }
            }

            SetConnectionStringAttribute();
        }
    }

    public class SitecoreWebconfigMerger : IInstallationStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;

        public SitecoreWebconfigMerger(SitecoreVersionChecker sitecoreVersionChecker)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
        }
        public void Execute()
        {
            var mergeConfig = new MergeConfig(
                "~/web.config",
                new List<Transformation>(){
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/CleanConfig.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.IIS7.config", isIntegrated: true),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.dependencies.sitecore.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/sitecore.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/ClientDependency.config")
                }
            );

            if (_sitecoreVersionChecker.IsLowerThan(new Version(8, 0)))
            {
                mergeConfig.Transformations.Add(
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/log4net.config"));
            }

            if (_sitecoreVersionChecker.IsLowerThan(new Version(8, 1))) // Only add new assembly bindings for version 8.0 and earlier.
            {
                mergeConfig.Transformations.Add(
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/updateAssemblyBinding.config"));
            }

            mergeConfig.Execute();
        }
    }
}