using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class UpdateUCommerceAssemblyVersionInDatabase : IStep
    {
        private readonly RuntimeVersionChecker _runtimeVersion;
        private readonly UpdateService _updateService;

        public UpdateUCommerceAssemblyVersionInDatabase(UpdateService updateService,
            RuntimeVersionChecker runtimeVersion)
        {
            _updateService = updateService;
            _runtimeVersion = runtimeVersion;
        }

        public async Task Run()
        {
            var assemblyVersion = _runtimeVersion.GetUcommerceRuntimeAssemblyVersion().ToString();
            _updateService.UpdateAssemblyVersion(assemblyVersion);
        }
    }
}