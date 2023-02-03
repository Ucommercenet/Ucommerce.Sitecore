using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class RenameConfigDefaultFilesToConfigFilesStep : IStep
    {
        private readonly bool _backupTarget;
        private readonly ConfigFileRenamer _command;
        private readonly IInstallerLoggingService _loggingService;
        private readonly DirectoryInfo _sourceDirectory;

        public RenameConfigDefaultFilesToConfigFilesStep(DirectoryInfo sourceDirectory, bool backupTarget, IInstallerLoggingService loggingService)
        {
            _backupTarget = backupTarget;
            _loggingService = loggingService;
            _command = new ConfigFileRenamer(sourceDirectory);
            _sourceDirectory = sourceDirectory;
        }

        public Task Run()
        {
            _loggingService.Information<RenameConfigDefaultFilesToConfigFilesStep>($"Renaming config.default files to config in {_sourceDirectory.FullName}...");
            _command.Rename(_backupTarget, ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
