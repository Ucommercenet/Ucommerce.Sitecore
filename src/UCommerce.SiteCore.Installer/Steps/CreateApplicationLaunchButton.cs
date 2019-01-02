using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
	/*Create the uCommerce launch button for SC8*/
	class CreateApplicationLaunchButton : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new InstallationSteps.CreateApplicationLaunchButton().Execute();
		}

		
	}
}
