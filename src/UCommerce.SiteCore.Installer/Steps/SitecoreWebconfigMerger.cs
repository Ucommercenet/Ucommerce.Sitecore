using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class SitecoreWebconfigMerger : IPostStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public SitecoreWebconfigMerger(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var mergeConfig = new MergeConfig(
                "~/web.config",
                new List<Transformation>(){
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/CleanConfig.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.IIS7.config", isIntegrated: true),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.dependencies.sitecore.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/sitecore.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/ClientDependency.config"),
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/updateAssemblyBinding.config")
                }, _loggingService
            );

            mergeConfig.Run(output, metaData);
        }
    }
}
