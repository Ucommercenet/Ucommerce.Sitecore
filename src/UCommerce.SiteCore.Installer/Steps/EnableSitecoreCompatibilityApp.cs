using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class EnableSitecoreCompatibilityApp : IStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;
        private readonly IInstallerLoggingService _sitecoreInstallerLoggingService;
        private readonly DirectoryInfo _baseDirectory;

        public EnableSitecoreCompatibilityApp(SitecoreVersionChecker sitecoreVersionChecker, DirectoryInfo baseDirectory,
            IInstallerLoggingService sitecoreInstallerLoggingService)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
            _sitecoreInstallerLoggingService = sitecoreInstallerLoggingService;
            _baseDirectory = baseDirectory;
        }

        public async Task Run()
        {
            _sitecoreInstallerLoggingService.Information<EnableSitecoreCompatibilityApp>("Enable sitecore compatibility app");
            var _virtualPathToAppsFolder = new DirectoryInfo(Path.Combine(_baseDirectory.FullName, "sitecore modules", "shell", "ucommerce", "apps"));
            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 2)))
            {
                new DirectoryMover(
                    new DirectoryInfo(
                        Path.Combine(_virtualPathToAppsFolder.FullName, "Sitecore92compatibility.disabled")),
                    new DirectoryInfo(
                       Path.Combine(_virtualPathToAppsFolder.FullName, "Sitecore92compatibility")),
                    true).Move(ex => _sitecoreInstallerLoggingService.Error<Exception>(ex));
            }


            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {

                new DirectoryMover(
                    new DirectoryInfo(
                        Path.Combine(_virtualPathToAppsFolder.FullName, "Sitecore93compatibility.disabled")),
                    new DirectoryInfo(
                      Path.Combine(_virtualPathToAppsFolder.FullName, "Sitecore93compatibility")),
                    true).Move(ex => _sitecoreInstallerLoggingService.Error<Exception>(ex));
            }
        }
    }
}