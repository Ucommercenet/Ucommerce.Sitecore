﻿using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Installation step that moves a file, if the target does not already exist
    /// </summary>
    public class MoveFileIfDoesntExist : IStep
    {
        private readonly FileMover _command;
        private readonly IInstallerLoggingService _loggingService;
        private readonly FileInfo _source;
        private readonly FileInfo _target;

        public MoveFileIfDoesntExist(FileInfo sourceFile, FileInfo targetFile, IInstallerLoggingService loggingService)
        {
            _source = sourceFile;
            _target = targetFile;
            _command = new FileMover(sourceFile, targetFile);
            _loggingService = loggingService;
        }

        public Task Run()
        {
            _loggingService.Information<MoveFileIfDoesntExist>($"Moving {_source.FullName} to {_target.FullName} if it does not already exist");
            _command.MoveIfDoesntExist(ex => _loggingService.Error<int>(ex));
            return Task.CompletedTask;
        }
    }
}
