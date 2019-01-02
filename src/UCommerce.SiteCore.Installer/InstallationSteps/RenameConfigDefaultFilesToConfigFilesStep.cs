using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class RenameConfigDefaultFilesToConfigFilesStep : IInstallationStep
    {
        private readonly bool _backupTarget;
        private readonly ConfigFileRenamer _command;

        public RenameConfigDefaultFilesToConfigFilesStep(string sourceVirtualPath, bool backupTarget)
        {
            _backupTarget = backupTarget;

            DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath(sourceVirtualPath));

            _command = new ConfigFileRenamer(directoryInfo);
        }
        public void Execute()
        {
            _command.Rename(_backupTarget, ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }
}