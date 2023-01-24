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

        public Task Run()
        {
            _sitecoreInstallerLoggingService.Information<EnableSitecoreCompatibilityApp>("Enabling sitecore compatibility apps");
            var pathToAppsFolder = new DirectoryInfo(Path.Combine(_sitecorePath.FullName, "sitecore modules", "shell", "ucommerce", "apps"));
            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 2)))
            {
                _sitecoreInstallerLoggingService.Information<EnableSitecoreCompatibilityApp>("Enabling sitecore 9.2+ compatibility app");
                new DirectoryMover(
                    new DirectoryInfo(Path.Combine(pathToAppsFolder.FullName, "Sitecore92compatibility.disabled")),
                    new DirectoryInfo(Path.Combine(pathToAppsFolder.FullName, "Sitecore92compatibility")),
                    true
                    )
                    .Move(ex => _sitecoreInstallerLoggingService.Error<Exception>(ex));
            }

            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {
                _sitecoreInstallerLoggingService.Information<EnableSitecoreCompatibilityApp>("Enabling sitecore 9.3+ compatibility app");
                new DirectoryMover(
                    new DirectoryInfo(Path.Combine(pathToAppsFolder.FullName, "Sitecore93compatibility.disabled")),
                    new DirectoryInfo(Path.Combine(pathToAppsFolder.FullName, "Sitecore93compatibility")),
                    true
                    )
                    .Move(ex => _sitecoreInstallerLoggingService.Error<Exception>(ex));
            }

            return Task.CompletedTask;
        }
    }
}
