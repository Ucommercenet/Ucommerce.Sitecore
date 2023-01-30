using System;
using System.Collections.Generic;
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
            var configIncludePath = Path.Combine("sitecore modules", "Shell", "ucommerce", "install", "configInclude");
            var appIncludePath = Path.Combine("App_Config", "include");

            Steps.AddRange(new IStep[]
            {
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, configIncludePath, "Sitecore.uCommerce.Databases.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Databases.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, configIncludePath, "Sitecore.uCommerce.Dataproviders.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Dataproviders.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, configIncludePath, "Sitecore.uCommerce.Events.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Events.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, configIncludePath, "Sitecore.uCommerce.Sites.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Sites.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                        configIncludePath,
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                        appIncludePath,
                        "Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config")),
                    true,
                    _loggingService)
            });

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {
                Steps.Add(
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                        true,
                        _loggingService)
                );
            }
            else
            {
                Steps.Add(
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.HttpRequestBegin.config")),
                        true,
                        _loggingService)
                );
            }

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 1)))
            {
                Steps.Add(
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.PreProcessRequest.9.1.config")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                        true,
                        _loggingService)
                );
            }
            else
            {
                Steps.Add(
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.PreProcessRequest.config")),
                        true,
                        _loggingService)
                );
            }

            Steps.AddRange(new List<IStep>
                {
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, configIncludePath, "Sitecore.uCommerce.Settings.config")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Settings.config")),
                        true,
                        _loggingService),
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                        true,
                        _loggingService),
                    new MoveFileIfTargetExist(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            appIncludePath,
                            "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config")),
                        true,
                        _loggingService),
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled")),
                        true,
                        _loggingService),
                }
            );

            if (versionChecker.IsEqualOrGreaterThan(new Version(8, 2)))
            {
                Steps.Add(
                    new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                            configIncludePath,
                            "Sitecore.uCommerce.WebApiConfiguration.config.disabled")),
                        new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.WebApiConfiguration.config")),
                        true,
                        _loggingService)
                );
            }

            Steps.AddRange(new List<IStep>
            {
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                        configIncludePath,
                        "Sitecore.uCommerce.initialize.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.initialize.config")),
                    true,
                    _loggingService),
                new MoveFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                        configIncludePath,
                        "Sitecore.uCommerce.Log4net.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, appIncludePath, "Sitecore.uCommerce.Log4net.config")),
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
