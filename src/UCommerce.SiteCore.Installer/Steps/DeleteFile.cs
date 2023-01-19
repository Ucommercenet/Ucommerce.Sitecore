using System;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class DeleteFile : IStep
    {
        private readonly FileDeleter _command;
        private readonly IInstallerLoggingService _loggingService;

        public DeleteFile(FileInfo file, IInstallerLoggingService loggingService)
        {
            _command = new FileDeleter(file);
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            _command.Delete(ex => _loggingService.Error<int>(ex));
        }
    }
}