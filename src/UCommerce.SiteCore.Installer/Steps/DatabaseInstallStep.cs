using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DatabaseInstallStep : IStep
    {
        private readonly List<IStep> _steps;

        public DatabaseInstallStep(SitecoreInstallationConnectionStringLocator connectionStringLocator,
            DirectoryInfo baseDirectory,
            IInstallerLoggingService logging,
            UpdateService updateService,
            RuntimeVersionChecker runtimeVersionChecker)
        {
            _steps = new List<IStep>
            {
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, logging),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, logging),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, logging)
            };
        }

        public async Task Run()
        {
            foreach (var step in _steps)
            {
                await step.Run();
            }
        }
    }
}
