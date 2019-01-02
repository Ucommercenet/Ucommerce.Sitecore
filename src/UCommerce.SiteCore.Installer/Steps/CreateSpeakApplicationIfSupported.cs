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
            new InstallationSteps.CreateSpeakApplicationIfSupported(_sitecoreVersionChecker).Execute();
        }
    }
}
