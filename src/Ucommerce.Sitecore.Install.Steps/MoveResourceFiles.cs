using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class MoveResourceFiles : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        public DirectoryInfo SourceDirectory { get; set; }
        public DirectoryInfo TargetDirectory { get; set; }

        public MoveResourceFiles(DirectoryInfo sitecoreDirectory, InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService logging)
        {
            SourceDirectory = sitecoreDirectory.CombineDirectory("bin", "uCommerce", "App_GlobalResources");
            TargetDirectory = sitecoreDirectory.CombineDirectory("App_GlobalResources");
            _loggingService = logging;
        }

        public async Task Run()
        {
            _loggingService.Information<MoveResourceFiles>($"Moving resource files from {SourceDirectory.FullName}...");
            var steps = new List<IStep>();
            var files = new[]
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
                steps.Add(new MoveFile(SourceDirectory.CombineFile(file),
                    TargetDirectory.CombineFile(file),
                    false,
                    _loggingService));
            }

            foreach (var step in steps)
            {
                await step.Run();
            }
        }
    }
}
