using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DatabaseInstallStep : AggregateStep
    {
        public DatabaseInstallStep(SitecoreInstallationConnectionStringLocator connectionStringLocator,
            DirectoryInfo baseDirectory,
            IInstallerLoggingService logging,
            UpdateService updateService,
            RuntimeVersionChecker runtimeVersionChecker)
        {
            Steps.AddRange(new IStep[]
            {
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, logging),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, logging),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, logging)
            });
        }
    }
}
