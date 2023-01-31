using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DeleteFile : IStep
    {
        private readonly FileDeleter _command;
        private readonly FileInfo _file;
        private readonly IInstallerLoggingService _loggingService;

        public DeleteFile(FileInfo file, IInstallerLoggingService loggingService)
        {
            _command = new FileDeleter(file);
            _file = file;
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<DeleteFile>($"Deleting file {_file.FullName}");
            _command.Delete(ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
