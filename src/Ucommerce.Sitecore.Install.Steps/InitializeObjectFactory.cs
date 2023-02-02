using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class InitializeObjectFactory : IStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public InitializeObjectFactory(IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<InitializeObjectFactory>("Initializing object factory...");
            var initializer = new ObjectFactoryInitializer();
            initializer.InitializeObjectFactory();
            return Task.CompletedTask;
        }
    }
}
