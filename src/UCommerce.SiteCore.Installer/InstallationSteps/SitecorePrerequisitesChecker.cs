using System.Collections.Generic;
using Sitecore.Install.Utils;
using Sitecore.IO;
using UCommerce.Installer;
using UCommerce.Installer.Prerequisites;
using UCommerce.Installer.Prerequisites.impl;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class SitecorePrerequisitesChecker : IInstallationStep
    {
        public void Execute()
        {
            var connectionStringLocator = new SitecoreInstallationConnectionStringLocator();

            var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();

            var steps = new List<IPrerequisitStep>()
            {
                new CanCreateTables(connectionStringLocator.LocateConnectionString(), sitecoreInstallerLoggingService),
                new CanModifyFiles(sitecoreInstallerLoggingService,FileUtil.MapPath("/")),
            };

            var checker = new PrerequisitesChecker(steps, new SitecoreInstallerLoggingService());

            string information;

            var meetsRequirements = checker.MeetsRequirement(out information);

            if (!meetsRequirements) throw new InstallationException(information);
        }
    }
}