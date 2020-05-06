using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveResourceFiles : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var postInstallationSteps = new List<IPostStep>();
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

            foreach (var file in files)
            {
                postInstallationSteps.Add(new MoveFile(string.Format("~/bin/uCommerce/App_GlobalResources/{0}", file), string.Format("~/App_GlobalResources/{0}", file), false));
            }

            foreach (var postInstallationStep in postInstallationSteps)
            {
                postInstallationStep.Run(output, metaData);
            }
        }
    }
}
