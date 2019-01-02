using System.Collections.Specialized;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
    public class CreateApplicationShortcuts : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
            new InstallationSteps.CreateApplicationShortcuts().Execute();
        }
	}
}