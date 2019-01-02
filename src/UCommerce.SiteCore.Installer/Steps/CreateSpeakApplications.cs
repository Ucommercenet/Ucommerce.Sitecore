using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Install.Framework;
using Sitecore.IO;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
    /// <summary>
    /// Setup the order management application and add to the launchpad.
    /// </summary>
    public class CreateSpeakApplications : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            new InstallationSteps.CreateSpeakApplications().Execute();
        }
    }
}
