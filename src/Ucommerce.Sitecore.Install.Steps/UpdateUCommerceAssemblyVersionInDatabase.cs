using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that updates the Ucommerce assembly version in the database
    /// </summary>
    public class UpdateUCommerceAssemblyVersionInDatabase : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly RuntimeVersionChecker _runtimeVersion;
        private readonly UpdateService _updateService;

        public UpdateUCommerceAssemblyVersionInDatabase(UpdateService updateService,
            RuntimeVersionChecker runtimeVersion,
            IInstallerLoggingService loggingService)
        {
            _updateService = updateService;
            _runtimeVersion = runtimeVersion;
            _loggingService = loggingService;
        }

        public Task Run()
        {
            var assemblyVersion = _runtimeVersion.GetUcommerceRuntimeAssemblyVersion()
                .ToString();
            _loggingService.Information<UpdateUCommerceAssemblyVersionInDatabase>($"Updating Ucommerce assembly version in database to {assemblyVersion}");
            _updateService.UpdateAssemblyVersion(assemblyVersion);
            return Task.CompletedTask;
        }
    }
}
