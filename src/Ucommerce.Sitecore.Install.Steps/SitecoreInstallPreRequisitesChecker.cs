using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class SitecoreInstallPreRequisitesChecker : IStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public SitecoreInstallPreRequisitesChecker(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<SitecoreInstallPreRequisitesChecker>("Checking if install prerequisites are met.");
            var steps = new List<IPrerequisitStep>
            {
                new CanModifyFiles(_loggingService, "/")
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
