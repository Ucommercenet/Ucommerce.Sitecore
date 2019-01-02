using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MoveDirectoryIfTargetExist : IInstallationStep
    {
        private readonly string _sourceDirectory;
        private readonly string _targetDirectory;

        public MoveDirectoryIfTargetExist(string sourceDirectory, string targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public void Execute()
        {
            new DirectoryMoverIfTargetExist(
                    new DirectoryInfo(HostingEnvironment.MapPath(_sourceDirectory)),
                    new DirectoryInfo(HostingEnvironment.MapPath(_targetDirectory)))
                .Move(ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }
}