using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class EnableSitecoreCompatibilityApp : IPostStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;
        private readonly IInstallerLoggingService _sitecoreInstallerLoggingService;

        public EnableSitecoreCompatibilityApp(SitecoreVersionChecker sitecoreVersionChecker, IInstallerLoggingService sitecoreInstallerLoggingService)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
            _sitecoreInstallerLoggingService = sitecoreInstallerLoggingService;
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            if (!_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(9, 2)))
            {
                return;
            }

            string _virtualPathToAppsFolder = "~/sitecore modules/shell/ucommerce/apps";

            new DirectoryMover(
                new DirectoryInfo(HostingEnvironment.MapPath($"{_virtualPathToAppsFolder}/Sitecore92compatibility.disabled")),
                new DirectoryInfo(HostingEnvironment.MapPath($"{_virtualPathToAppsFolder}/Sitecore92compatibility")),
                true).Move(ex => _sitecoreInstallerLoggingService.Log<Exception>(ex));
        }
    }
}