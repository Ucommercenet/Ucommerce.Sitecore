using System;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that moves a file, if the given function returns true
    /// </summary>
    public class MoveFileIf : IStep
    {
        private readonly Func<bool> _condition;
        private readonly IInstallerLoggingService _loggingService;
        private readonly MoveFile _moveStep;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public MoveFileIf(FileInfo source,
            FileInfo target,
            bool backupTarget,
            Func<bool> condition,
            IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _condition = condition;
            _moveStep = new MoveFile(source, target, backupTarget, loggingService);
            _source = source;
            _target = target;
        }

        public async Task Run()
        {
            _loggingService.Information<MoveFileIf>($"Moving {_source.FullName} to {_target.FullName} if condition is met");
            if (_condition())
            {
                await _moveStep.Run();
            }
        }
    }
}
