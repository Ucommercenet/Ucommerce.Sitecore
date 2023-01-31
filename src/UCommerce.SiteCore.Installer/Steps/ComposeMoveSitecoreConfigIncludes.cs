using System;
using System.IO;
using Ucommerce.Installer;

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
                new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "ucommerce", "install", "configInclude"));
            var appIncludeDirectory = new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "App_Config", "include"));

            Steps.AddRange(new IStep[]
            {
                new MoveFile(new FileInfo(Path.Combine(configIncludeDirectory.FullName, "Sitecore.uCommerce.Databases.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Databases.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(configIncludeDirectory.FullName, "Sitecore.uCommerce.Dataproviders.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Dataproviders.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(configIncludeDirectory.FullName, "Sitecore.uCommerce.Events.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Events.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(configIncludeDirectory.FullName, "Sitecore.uCommerce.Sites.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Sites.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config")),
                    new FileInfo(Path.Combine(
                        appIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                    true,
                    _loggingService),
                new MoveFileIf(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                    true,
                    () => versionChecker.IsEqualOrGreaterThan(new Version(9, 3)),
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                    true,
                    _loggingService),
                new MoveFileIf(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.PreProcessRequest.9.1.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                    true,
                    () => versionChecker.IsEqualOrGreaterThan(new Version(9, 1)),
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(configIncludeDirectory.FullName, "Sitecore.uCommerce.Settings.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Settings.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                    true,
                    _loggingService),
                new MoveFileIfTargetExist(new FileInfo(Path.Combine(
                        appIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                    true,
                    _loggingService),
                new MoveFileIf(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.WebApiConfiguration.config.disabled")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.WebApiConfiguration.config")),
                    true,
                    () => versionChecker.IsLowerThan(new Version(8, 2)),
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.initialize.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.initialize.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(
                        configIncludeDirectory.FullName,
                        "Sitecore.uCommerce.Log4net.config")),
                    new FileInfo(Path.Combine(appIncludeDirectory.FullName, "Sitecore.uCommerce.Log4net.config")),
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
