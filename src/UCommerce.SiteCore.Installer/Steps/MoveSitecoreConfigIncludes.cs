using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveSitecoreConfigIncludes : IPostStep
    {
        private readonly SitecoreVersionChecker _versionChecker;

        public MoveSitecoreConfigIncludes(SitecoreVersionChecker versionChecker)
        {
            _versionChecker = versionChecker;
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var steps = new List<IPostStep>
            {
                new MoveFileWeb(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Databases.config",
                    "~/App_Config/include/Sitecore.uCommerce.Databases.config",
                    backupTarget: false),
                new MoveFileWeb(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Dataproviders.config",
                    "~/App_Config/include/Sitecore.uCommerce.Dataproviders.config",
                    backupTarget: false),
                new MoveFileWeb(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.initialize.config",
                    "~/App_Config/include/Sitecore.uCommerce.initialize.config",
                    backupTarget: false)
            };

            if (_versionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {
                steps.Add(new MoveFileWeb(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    backupTarget: false));
            }
            else
            {
                steps.Add(new MoveFileWeb(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    backupTarget: false));
            }

            foreach (var step in steps)
            {
                step.Run(output, metaData);
            }
        }
    }
}
