using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Sitecore.Install.Utils;
using Sitecore.IO;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class SitecorePreRequisitesChecker : IPostStep
	{
        private readonly IInstallerLoggingService _loggingService;

        public SitecorePreRequisitesChecker(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			var connectionStringLocator = new SitecoreInstallationConnectionStringLocator();

			var steps = new List<IPrerequisitStep>()
				{
					new CanCreateTables(connectionStringLocator.LocateConnectionString(), _loggingService),
					new CanModifyFiles(_loggingService, FileUtil.MapPath("/")),
				};

			var checker = new PrerequisitesChecker(steps, _loggingService);

            var meetsRequirements = checker.MeetsRequirement(out var information);

			if (!meetsRequirements) throw new InstallationException(information);
		}
	}
}
