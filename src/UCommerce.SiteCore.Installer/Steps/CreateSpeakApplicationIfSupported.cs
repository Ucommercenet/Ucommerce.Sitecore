using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
    public class CreateSpeakApplicationIfSupported : IPostStep
    {
        private readonly SitecoreVersionChecker _sitecoreVersionChecker;

        public CreateSpeakApplicationIfSupported(SitecoreVersionChecker sitecoreVersionChecker)
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
