using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class SitecoreDatabasePreRequisitesChecker : IStep
    {
        private readonly InstallationConnectionStringLocator _connectionStringLocator;
        private readonly IInstallerLoggingService _loggingService;

        public SitecoreDatabasePreRequisitesChecker(InstallationConnectionStringLocator connectionStringLocator, IInstallerLoggingService loggingService)
        {
            _connectionStringLocator = connectionStringLocator;
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<SitecoreDatabasePreRequisitesChecker>("Checking if database prerequisites are met.");
            var steps = new List<IPrerequisitStep>
            {
                new CanCreateTables(_connectionStringLocator.LocateConnectionString(), _loggingService)
            };

            var checker = new PrerequisitesChecker(steps, _loggingService);
            var meetsRequirements = checker.MeetsRequirement(out var information);

            if (!meetsRequirements)
            {
                _loggingService.Error<PrerequisitesChecker>(new Exception(information));
            }

            return Task.CompletedTask;
        }
    }
}
