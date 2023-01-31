using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveFile : IStep
    {
        private readonly bool _backupTarget;
        private readonly FileMover _command;
        private readonly IInstallerLoggingService _loggingService;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public MoveFile(FileInfo source, FileInfo target, bool backupTarget, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _backupTarget = backupTarget;
            _source = source;
            _target = target;
            _command = new FileMover(source, target);
        }

        public Task Run()
        {
            _loggingService.Information<MoveFile>($"Moving file {_source.FullName} to {_target.FullName}");
            _command.Move(_backupTarget, ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
