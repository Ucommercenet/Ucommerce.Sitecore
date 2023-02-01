﻿using System;
using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class ComposeMoveSitecoreConfigIncludes : AggregateStep
    {
        private readonly IInstallerLoggingService _loggingService;

        public ComposeMoveSitecoreConfigIncludes(DirectoryInfo sitecoreDirectory,
            ISitecoreVersionChecker versionChecker,
            IInstallerLoggingService loggingService)
        {
            _loggingService = loggingService;
            var configIncludeDirectory =
                sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "ucommerce", "install", "configInclude");
            var appIncludeDirectory = sitecoreDirectory.CombineDirectory("App_Config", "include");

            Steps.AddRange(new IStep[]
            {
                new MoveFile(configIncludeDirectory.CombineFile("Sitecore.uCommerce.Databases.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Databases.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile("Sitecore.uCommerce.Dataproviders.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Dataproviders.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile("Sitecore.uCommerce.Events.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Events.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile("Sitecore.uCommerce.Sites.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Sites.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config"),
                    appIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.HttpRequestBegin.config"),
                    true,
                    _loggingService),
                new MoveFileIf(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.HttpRequestBegin.config"),
                    true,
                    () => versionChecker.IsEqualOrGreaterThan(new Version(9, 3)),
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.PreProcessRequest.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.PreProcessRequest.config"),
                    true,
                    _loggingService),
                new MoveFileIf(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.PreProcessRequest.9.1.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.PreProcessRequest.config"),
                    true,
                    () => versionChecker.IsEqualOrGreaterThan(new Version(9, 1)),
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile("Sitecore.uCommerce.Settings.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Settings.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled"),
                    true,
                    _loggingService),
                new MoveFileIfTargetExist(appIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.ModifyPipelines.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled"),
                    true,
                    _loggingService),
                new MoveFileIf(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.WebApiConfiguration.config.disabled"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.WebApiConfiguration.config"),
                    true,
                    () => versionChecker.IsLowerThan(new Version(8, 2)),
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.initialize.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.initialize.config"),
                    true,
                    _loggingService),
                new MoveFile(configIncludeDirectory.CombineFile(
                        "Sitecore.uCommerce.Log4net.config"),
                    appIncludeDirectory.CombineFile("Sitecore.uCommerce.Log4net.config"),
                    true,
                    _loggingService),
            });
        }

        protected override void LogStart()
        {
            _loggingService.Information<ComposeMoveSitecoreConfigIncludes>("Moving Sitecore configs...");
        }
    }
}
