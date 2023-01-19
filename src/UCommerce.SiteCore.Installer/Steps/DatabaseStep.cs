using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DatabaseStep : IStep
    {
        private readonly List<IStep> _steps;

        public DatabaseStep(string connectionString, bool backupDb = false, bool upgradeDb = false)
        {
            _steps = AddSteps(connectionString, backupDb, upgradeDb);
        }

        public Task Run()
        {
            foreach (var step in _steps) step.Run();
            return Task.CompletedTask;
        }

        private List<IStep> AddSteps(string connectionString, bool backupDb, bool upgradeDb)
        {
            var steps = new List<IStep>();
            if (upgradeDb)
            {
                //  steps.Add(new InstallDatabase("~/sitecore modules/Shell/ucommerce/install"));
                //  steps.Add(new InstallDatabaseSitecore("~/sitecore modules/Shell/ucommerce/install"));
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