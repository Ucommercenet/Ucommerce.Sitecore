using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Install.Framework;
using Sitecore.IO;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class AddTitleToCommerceSpeakAppsSection : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			new InstallationSteps.AddTitleToCommerceSpeakAppsSection().Execute();
		}
		
	
	}
}
