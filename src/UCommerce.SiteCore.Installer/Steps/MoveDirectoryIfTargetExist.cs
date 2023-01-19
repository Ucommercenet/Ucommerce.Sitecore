using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveDirectoryIfTargetExist : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly DirectoryInfo _sourceDirectory;
        private readonly DirectoryInfo _targetDirectory;

        public MoveDirectoryIfTargetExist(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, IInstallerLoggingService loggingService)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _loggingService = loggingService;
        }

        public async Task Run()
        {
            _loggingService.Information<MoveDirectoryIfTargetExist>($"Moving {_sourceDirectory} to {_targetDirectory} if it exists");
            new DirectoryMoverIfTargetExist(_sourceDirectory, _targetDirectory)
                .Move(ex => new SitecoreInstallerLoggingService().Error<int>(ex));
        }
    }
}
