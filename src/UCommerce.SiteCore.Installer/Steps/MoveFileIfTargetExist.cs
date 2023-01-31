using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveFileIfTargetExist : IStep
    {
        private readonly bool _backupTarget;
        private readonly FileMoverIfTargetExist _command;
        private readonly IInstallerLoggingService _loggingService;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public MoveFileIfTargetExist(FileInfo source, FileInfo target, bool backupTarget, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _backupTarget = backupTarget;
            _source = source;
            _target = target;
            _command = new FileMoverIfTargetExist(source, target);
        }

        public Task Run()
        {
            _loggingService.Information<MoveFileIfTargetExist>($"Moving {_source.FullName} to {_target.FullName} if it exists");
            _command.MoveIfTargetExist(_backupTarget, ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
