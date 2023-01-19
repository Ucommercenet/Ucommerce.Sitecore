using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class RenameConfigDefaultFilesToConfigFilesStep : IStep
    {
        private readonly bool _backupTarget;
        private readonly ConfigFileRenamer _command;

        public RenameConfigDefaultFilesToConfigFilesStep(DirectoryInfo sourceVDirectory, bool backupTarget)
        {
            _backupTarget = backupTarget;

            _command = new ConfigFileRenamer(sourceVDirectory);
        }

        public async Task Run()
        {
            _command.Rename(_backupTarget, ex => new SitecoreInstallerLoggingService().Error<int>(ex));
        }
    }
}
