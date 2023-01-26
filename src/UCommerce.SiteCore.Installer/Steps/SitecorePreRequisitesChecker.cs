using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Install.Utils;
using Sitecore.IO;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class SitecorePreRequisitesChecker : IStep
    {
        private readonly InstallationConnectionStringLocator _connectionStringLocator;
        private readonly IInstallerLoggingService _loggingService;

        public SitecorePreRequisitesChecker(InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService loggingService)
        {
            _connectionStringLocator = connectionStringLocator;
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            _loggingService.Information<SitecorePreRequisitesChecker>("Checking if prerequisites are met.");

            var steps = new List<IPrerequisitStep>
            {
                new CanCreateTables(_connectionStringLocator.LocateConnectionString(), _loggingService),
                new CanModifyFiles(_loggingService, FileUtil.MapPath("/"))
            };

            var checker = new PrerequisitesChecker(steps, _loggingService);

            var meetsRequirements = checker.MeetsRequirement(out var information);

            if (!meetsRequirements) _loggingService.Error<PrerequisitesChecker>(new InstallationException(information));
        }
    }
}
