using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class RenameConfigDefaultFilesToConfigFilesStep : IPostStep
    {
        private readonly bool _backupTarget;
        private readonly IInstallerLoggingService _installerLoggingService;
        private readonly ConfigFileRenamer _command;

        public RenameConfigDefaultFilesToConfigFilesStep(string sourceVirtualPath, bool backupTarget, IInstallerLoggingService installerLoggingService)
        {
            _backupTarget = backupTarget;
            _installerLoggingService = installerLoggingService;

            DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath(sourceVirtualPath));

            _command = new ConfigFileRenamer(directoryInfo);
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            _command.Rename(_backupTarget, ex => _installerLoggingService.Error<RenameConfigDefaultFilesToConfigFilesStep>(ex));
        }
    }
}
