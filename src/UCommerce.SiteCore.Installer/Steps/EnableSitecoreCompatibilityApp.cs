using System;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class EnableSitecoreCompatibilityApp : IStep
    {
        private readonly DirectoryInfo _sitecorePath;
        private readonly IInstallerLoggingService _sitecoreInstallerLoggingService;
        private readonly ISitecoreVersionChecker _sitecoreVersionChecker;

        public EnableSitecoreCompatibilityApp(ISitecoreVersionChecker sitecoreVersionChecker,
            DirectoryInfo sitecorePath,
            IInstallerLoggingService sitecoreInstallerLoggingService)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
            _sitecoreInstallerLoggingService = sitecoreInstallerLoggingService;
            _sitecorePath = sitecorePath;
        }

        public async Task Run()
        {
            _sitecoreInstallerLoggingService.Information<EnableSitecoreCompatibilityApp>("Enabling sitecore compatibility apps");
            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 2)))
            {
                await new EnableApp("Sitecore92compatibility", _sitecorePath, _sitecoreInstallerLoggingService).Run();
            }

            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {
                await new EnableApp("Sitecore93compatibility", _sitecorePath, _sitecoreInstallerLoggingService).Run();
            }
        }
    }
}
