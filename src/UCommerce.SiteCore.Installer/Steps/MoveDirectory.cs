using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    /// <summary>
    ///     Moves a folder.
    /// </summary>
    public class MoveDirectory : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly bool _overwriteTarget;
        private readonly DirectoryInfo _sourceDirectory;
        private readonly DirectoryInfo _targetDirectory;

        public MoveDirectory(DirectoryInfo sourceDirectory,
            DirectoryInfo targetDirectory,
            bool overwriteTarget,
            IInstallerLoggingService loggingService)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _overwriteTarget = overwriteTarget;
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            _loggingService.Information<MoveDirectory>($"Moving directory {_sourceDirectory} to {_targetDirectory}");
            new DirectoryMover(
                _sourceDirectory,
                _targetDirectory,
                _overwriteTarget).Move(
                ex => _loggingService.Error<int>(ex));
        }
    }
}
