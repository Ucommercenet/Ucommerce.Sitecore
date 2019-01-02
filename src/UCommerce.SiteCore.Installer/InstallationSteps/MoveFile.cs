using System.IO;
using System.Web.Hosting;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MoveFile : IInstallationStep
    {
        private readonly bool _backupTarget;
        private readonly UCommerce.Installer.FileMover _command;

        public MoveFile(string source, string target, bool backupTarget)
        {
            _backupTarget = backupTarget;
            
            FileInfo sourceFile = new FileInfo(HostingEnvironment.MapPath(source)), targetFile = new FileInfo(HostingEnvironment.MapPath(target));

            _command = new UCommerce.Installer.FileMover(sourceFile, targetFile);
        }

        public void Execute()
        {
            _command.Move(_backupTarget, ex => new SitecoreInstallerLoggingService().Log<int>(ex));
        }
    }
}