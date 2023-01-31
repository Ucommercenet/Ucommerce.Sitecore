using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class UpdateUCommerceAssemblyVersionInDatabase : IStep
    {
        private readonly RuntimeVersionChecker _runtimeVersion;
        private readonly UpdateService _updateService;
        private readonly IInstallerLoggingService _loggingService;

        public UpdateUCommerceAssemblyVersionInDatabase(UpdateService updateService,
            RuntimeVersionChecker runtimeVersion, IInstallerLoggingService loggingService)
        {
            _updateService = updateService;
            _runtimeVersion = runtimeVersion;
            _loggingService = loggingService;
        }

        public Task Run()
        {
            var assemblyVersion = _runtimeVersion.GetUcommerceRuntimeAssemblyVersion().ToString();
            _loggingService.Information<UpdateUCommerceAssemblyVersionInDatabase>($"Updating Ucommerce assembly version in database to {assemblyVersion}");
            _updateService.UpdateAssemblyVersion(assemblyVersion);
            return Task.CompletedTask;
        }
        
    }
}