using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DeleteDirectory : IStep
    {
        private readonly DirectoryDeleter _command;
        private readonly IInstallerLoggingService _loggingService;

        public DeleteDirectory(DirectoryInfo directory, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _command = new DirectoryDeleter(directory);
        }

        public async Task Run()
        {
            _command.Delete(ex => _loggingService.Error<int>(ex));
        }
    }
}