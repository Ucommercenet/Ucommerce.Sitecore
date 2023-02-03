using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class DeleteDirectory : IStep
    {
        private readonly DirectoryDeleter _command;
        private readonly DirectoryInfo _directory;
        private readonly IInstallerLoggingService _loggingService;

        public DeleteDirectory(DirectoryInfo directory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _directory = directory;
            _command = new DirectoryDeleter(directory);
        }

        public Task Run()
        {
            _loggingService.Information<DeleteDirectory>($"Deleting directory {_directory.FullName}");
            _command.Delete(ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
