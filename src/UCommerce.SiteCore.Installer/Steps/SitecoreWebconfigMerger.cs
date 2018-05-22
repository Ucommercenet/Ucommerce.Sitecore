using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
    public class SitecoreWebconfigMerger : IPostStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;

        public SitecoreWebconfigMerger(SitecoreVersionChecker sitecoreVersionChecker)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
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
        
            mergeConfig.Run(output, metaData);
        }
    }
}
