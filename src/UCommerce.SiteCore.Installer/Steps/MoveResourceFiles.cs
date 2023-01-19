using Castle.DynamicProxy.Contributors;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Web.UI.Ucommerce.Resources.Sitecore8;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveResourceFiles : IStep
    {
        public DirectoryInfo SourceDirctory { get; set; }
        public DirectoryInfo TargetDirctory { get; set; }

        public MoveResourceFiles(DirectoryInfo basePath, IInstallerLoggingService logging, InstallationConnectionStringLocator connectionStringLocator )
        {
            SourceDirctory = new DirectoryInfo( Path.Combine(basePath.FullName, "bin", "uCommerce", "App_GlobalResources") );
            TargetDirctory = new DirectoryInfo( Path.Combine(basePath.FullName, "App_GlobalResources"));
        }
        public Task Run()
        {
            var postInstallationSteps = new List<IStep>();
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
                postInstallationSteps.Add(new MoveFile(new FileInfo(Path.Combine(SourceDirctory.FullName, file)), new FileInfo(Path.Combine(TargetDirctory.FullName, file)), false));
            }

            foreach (var postInstallationStep in postInstallationSteps)
            {
                postInstallationStep.Run();
            }
            return Task.CompletedTask;
        }
    }
}
