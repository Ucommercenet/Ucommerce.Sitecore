using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class BackupFile : IStep
    {
        private readonly FileBackup _command;
        private readonly FileInfo _file;
        private readonly IInstallerLoggingService _loggingService;

        public BackupFile(FileInfo file, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _command = new FileBackup(file);
            _file = file;
        }

        public Task Run()
        {
            _loggingService.Information<FileBackup>($"Backing up file: {_file.FullName}");
            _command.Backup(ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
