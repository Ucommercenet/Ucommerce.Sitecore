using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class CreateSpeakApplicationIfSupported : IPostStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;
        private readonly IInstallerLoggingService _installerLoggingService;

        public CreateSpeakApplicationIfSupported(SitecoreVersionChecker sitecoreVersionChecker, IInstallerLoggingService installerLoggingService)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
            _installerLoggingService = installerLoggingService;
        }
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var tasks = new List<IPostStep>();
            if (_sitecoreVersionChecker.SupportsSpeakApps())
            {
                tasks.Add(new CreateSpeakApplications(_installerLoggingService));
                tasks.Add(new CreateApplicationLaunchButton());

                //Remove uCommerce shortcut on desktop if present
                tasks.Add(new RemoveUCommerceApplicationLaunchButton());
            }

            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(8, 2)))
            {
                tasks.Add(new AddTitleToCommerceSpeakAppsSection(_installerLoggingService));
            }

            foreach (var task in tasks)
            {
                task.Run(output, metaData);
            }
        }
    }
}
