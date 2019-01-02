using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class InitializeObjectFactory : IInstallationStep
    {
        public void Execute()
        {
            var initializer = new ObjectFactoryInitializer();
            initializer.InitializeObjectFactory();
        }
    }
}