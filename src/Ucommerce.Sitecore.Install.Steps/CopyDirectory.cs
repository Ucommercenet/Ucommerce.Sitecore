using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class CopyDirectory : IStep
    {
        private readonly IInstallerLoggingService _loggingService;
        private readonly bool _recursive;
        private readonly DirectoryInfo _sourceDirectory;
        private readonly DirectoryInfo _targetDirectory;

        public CopyDirectory(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, bool recursive, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            _recursive = recursive;
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public Task Run()
        {
            Directory.CreateDirectory(_targetDirectory.FullName);
            CopyDir(_sourceDirectory, _targetDirectory, true, _loggingService);
            return Task.CompletedTask;
        }

        private static void CopyDir(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool recursive, IInstallerLoggingService loggingService)
        {
            if (!sourceDir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");
            loggingService.Information<CopyDirectory>(
                $"Copying directory {sourceDir.FullName} to {destinationDir.FullName}{(recursive == false ? "" : " and overwriting")}...");

            var subDirs = sourceDir.GetDirectories();

            Directory.CreateDirectory(destinationDir.FullName);

            foreach (var file in sourceDir.GetFiles())
            {
                var targetFilePath = destinationDir.CombineFile(file.Name)
                    .FullName;
                file.CopyTo(targetFilePath, true);

                var fs = file.GetAccessControl();
                fs.SetAccessRuleProtection(false, false);
                file.SetAccessControl(fs);
            }

            if (!recursive) return;
            foreach (var subDir in subDirs)
            {
                CopyDir(subDir, destinationDir.CombineDirectory(subDir.Name), true, loggingService);
            }
        }
    }
}
