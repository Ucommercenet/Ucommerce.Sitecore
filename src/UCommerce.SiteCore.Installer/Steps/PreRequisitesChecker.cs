using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Install.Utils;
using Sitecore.IO;
using Ucommerce.Installer;
using Ucommerce.Installer.Prerequisites;
using Ucommerce.Installer.Prerequisites.impl;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class PreRequisitesChecker : IStep
    {
        private readonly string _connectionString;
        private readonly IInstallerLoggingService _loggingService;

        public PreRequisitesChecker(string connectionString, IInstallerLoggingService loggingService)
        {
            _connectionString = connectionString;
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            var connectionStringLocator = new SitecoreInstallationConnectionStringLocator(_connectionString);

            var steps = new List<IPrerequisitStep>
            {
                new CanCreateTables(connectionStringLocator.LocateConnectionString(), _loggingService),
                new CanModifyFiles(_loggingService, FileUtil.MapPath("/"))
            };

            var checker = new PrerequisitesChecker(steps, _loggingService);

            var meetsRequirements = checker.MeetsRequirement(out var information);

            if (!meetsRequirements) throw new InstallationException(information);
        }
    }
}
