using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.IO;
using Sitecore.Install.Framework;
using Sitecore.Install.Utils;
using UCommerce.Installer.Prerequisites;
using UCommerce.Installer.Prerequisites.impl;
using UCommerce.Sitecore.Installer.Steps.PreRequisitesSteps;

namespace UCommerce.Sitecore.Installer.Steps
{
	public class SitecorePreRequisitesChecker : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			var connectionStringLocator = new SitecoreInstallationConnectionStringLocator();
			
			var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();
			
			var steps = new List<IPrerequisitStep>()
				{
					new CanCreateTables(connectionStringLocator.LocateConnectionString(), sitecoreInstallerLoggingService),
					new CanModifyFiles(sitecoreInstallerLoggingService,FileUtil.MapPath("/")),
					new NoConflictingAppHost()
				};

			var checker = new PrerequisitesChecker(steps,new SitecoreInstallerLoggingService());

			string information;

			var meetsRequirements = checker.MeetsRequirement(out information);

			if (!meetsRequirements) throw new InstallationException(information);
		}
	}
}
