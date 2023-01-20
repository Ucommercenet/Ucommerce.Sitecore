using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DatabaseStep : IStep
    {
        private readonly IInstallerLoggingService _logging;
        private readonly DirectoryInfo _baseDirectory;
        private readonly List<IStep> _steps;

        public DatabaseStep(string connectionString, DirectoryInfo baseDirectory, IInstallerLoggingService logging, bool backupDb = false, bool upgradeDb = false)
        {
            _logging = logging;
            _baseDirectory = baseDirectory;
            _steps = AddSteps(connectionString, backupDb, upgradeDb);
        }

        public async Task Run()
        {
            foreach (var step in _steps)
            {
                await step.Run();
            }
        }

        private List<IStep> AddSteps(string connectionString, bool backupDb, bool upgradeDb)
        {
            var steps = new List<IStep>();
            if (upgradeDb)
            {
                var connectionStringLocator = new SitecoreInstallationConnectionStringLocator(connectionString);
                steps.Add(new InstallDatabase(_baseDirectory, connectionStringLocator, _logging));
                steps.Add(new InstallDatabaseSitecore(_baseDirectory, connectionStringLocator, _logging));
                //  steps.Add(new UpdateUCommerceAssemblyVersionInDatabase(updateService,
                //      runtimeVersionChecker, sitecoreInstallerLoggingService));
            }

            if (backupDb)
            {
                //New steps
            }

            return steps;
        }
    }
}