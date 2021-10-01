using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveResourceFiles : IPostStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public MoveResourceFiles(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var files = new string[]
            {
                "Admin.da.resx",
                "Admin.de.resx",
                "Admin.resx",
                "Admin.sv.resx",
                "Definition.da.resx",
                "Definition.de.resx",
                "Definition.resx",
                "Definition.sv.resx",
                "Icons.resx",
                "RoleName.da.resx",
                "RoleName.resx",
                "Search.da.resx",
                "Search.de.resx",
                "Search.resx",
                "Search.sv.resx",
                "Tabs.da.resx",
                "Tabs.de.resx",
                "Tabs.resx",
                "Tabs.sv.resx",
                "OrdersCount.da.resx",
                "OrdersCount.de.resx",
                "OrdersCount.sv.resx",
                "OrdersCount.resx",
                "OrderList.da.resx",
                "OrderList.de.resx",
                "OrderList.sv.resx",
                "OrderList.resx",
                "CatalogSearch.da.resx",
                "CatalogSearch.de.resx",
                "CatalogSearch.sv.resx",
                "CatalogSearch.resx",
            };

            var postInstallationSteps = files.Select(file => new MoveFile($"~/bin/uCommerce/App_GlobalResources/{file}", $"~/App_GlobalResources/{file}", false, _loggingService))
                                             .Cast<IPostStep>()
                                             .ToList();

            foreach (var postInstallationStep in postInstallationSteps)
            {
                postInstallationStep.Run(output, metaData);
            }
        }
    }
}
