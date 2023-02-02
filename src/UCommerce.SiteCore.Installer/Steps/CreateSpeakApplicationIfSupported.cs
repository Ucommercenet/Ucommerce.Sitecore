using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Ucommerce.Sitecore.Install;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class CreateSpeakApplicationIfSupported : IPostStep
    {
        private readonly ISitecoreVersionChecker _sitecoreVersionChecker;

        public CreateSpeakApplicationIfSupported(ISitecoreVersionChecker sitecoreVersionChecker)
        {
            _sitecoreVersionChecker = sitecoreVersionChecker;
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var tasks = new List<IPostStep>();
            if (_sitecoreVersionChecker.SupportsSpeakApps())
            {
                tasks.Add(new CreateSpeakApplications());
                tasks.Add(new CreateApplicationLaunchButton());

                //Remove uCommerce shortcut on desktop if present
                tasks.Add(new RemoveUCommerceApplicationLaunchButton());
            }

            if (_sitecoreVersionChecker.IsEqualOrGreaterThan(new Version(8, 2)))
            {
                tasks.Add(new AddTitleToCommerceSpeakAppsSection());
            }

            foreach (var task in tasks)
            {
                task.Run(output, metaData);
            }
        }
    }
}
