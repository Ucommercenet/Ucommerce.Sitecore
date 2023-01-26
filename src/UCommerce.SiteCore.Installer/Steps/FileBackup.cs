using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class FileBackup : IStep
    {
        private readonly Ucommerce.Installer.FileBackup _command;
        private readonly FileInfo _file;
        private readonly IInstallerLoggingService _loggingService;

        public FileBackup(FileInfo file, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _command = new Ucommerce.Installer.FileBackup(file);
            _file = file;
        }

        public async Task Run()
        {
            _loggingService.Information<FileBackup>($"Backing up file: {_file.FullName}");
            _command.Backup(ex => _loggingService.Error<int>(ex));
        }
    }
}
