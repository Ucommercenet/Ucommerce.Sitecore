﻿using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate step that deletes RavenDB files, which are no longer used
    /// </summary>
    public class DeleteRavenDB : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public DeleteRavenDB(DirectoryInfo appsPath, IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            Steps.AddRange(new IStep[]
            {
                new DeleteDirectory(appsPath.CombineDirectory("RavenDB25"), _loggingService),
                new DeleteDirectory(appsPath.CombineDirectory("RavenDB25.disabled"), _loggingService),
                new DeleteDirectory(appsPath.CombineDirectory("RavenDB30"), _loggingService),
                new DeleteDirectory(appsPath.CombineDirectory("RavenDB30.disabled"), _loggingService),
            });
        }

        protected override void LogStart()
        {
            _loggingService.Information<DeleteRavenDB>("Deleting RavenDB since it is no longer used...");
        }
    }
}
