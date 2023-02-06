using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate step that moves resx files from the Ucommerce/App_GlobalResources to App_GlobalResources
    /// </summary>
    public class MoveResourceFiles : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private DirectoryInfo SourceDirectory { get; }
        private DirectoryInfo TargetDirectory { get; }

        public MoveResourceFiles(DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            SourceDirectory = sitecoreDirectory.CombineDirectory("bin", "uCommerce", "App_GlobalResources");
            TargetDirectory = sitecoreDirectory.CombineDirectory("App_GlobalResources");
            _loggingService = logging;
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
                Steps.Add(new MoveFile(SourceDirectory.CombineFile(file),
                    TargetDirectory.CombineFile(file),
                    false,
                    _loggingService));
            }
        }

        protected override void LogStart()
        {
            _loggingService.Information<MoveResourceFiles>($"Moving resource files from {SourceDirectory.FullName}...");
        }
    }
}
