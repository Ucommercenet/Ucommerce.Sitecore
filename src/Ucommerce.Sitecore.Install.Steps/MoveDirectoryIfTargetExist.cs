﻿using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that moves a given directory, if the target location exists
    /// </summary>
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

        public Task Run()
        {
            _loggingService.Information<MoveDirectoryIfTargetExist>($"Moving {_sourceDirectory.FullName} to {_targetDirectory.FullName} if it exists");
            new DirectoryMoverIfTargetExist(_sourceDirectory, _targetDirectory)
                .Move(ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
