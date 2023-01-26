using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class InitializeObjectFactory : IStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public InitializeObjectFactory(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            _loggingService.Information<InitializeObjectFactory>("Initializing object factory...");
            var initializer = new ObjectFactoryInitializer();
            initializer.InitializeObjectFactory();
        }
    }
}
