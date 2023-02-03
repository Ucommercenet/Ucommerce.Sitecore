using System;
using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;
using Ucommerce.Sitecore.Installer.Steps;

namespace Ucommerce.Sitecore.Install.Steps
{
    public class InstallStep : AggregateStep
    {
        public InstallStep(DirectoryInfo baseDirectory,
            DirectoryInfo sitecoreDirectory,
            ISitecoreVersionChecker versionChecker,
            InstallationConnectionStringLocator connectionStringLocator,
            UpdateService updateService,
            RuntimeVersionChecker runtimeVersionChecker,
            IInstallerLoggingService loggingService)
        {
            var appsDirectory = sitecoreDirectory.CombineDirectory("sitecore modules", "Shell", "uCommerce", "Apps");
            Steps.AddRange(new IStep[]
            {
                new SitecorePreRequisitesChecker(connectionStringLocator, loggingService),
                new InitializeObjectFactory(loggingService),
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, loggingService),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, loggingService),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, loggingService),
                new CopyDirectory(baseDirectory.CombineDirectory("package", "files"), sitecoreDirectory, true, loggingService),
                new CopyFile(sitecoreDirectory.CombineFile("web.config"),
                    sitecoreDirectory.CombineFile($"web.config.{DateTime.Now.Ticks}.backup"),
                    loggingService),
                new SitecoreWebconfigMerger(sitecoreDirectory, loggingService),
                new SeperateConfigSectionInNewFile("configuration/sitecore/settings",
                    sitecoreDirectory.CombineFile("web.config"),
                    sitecoreDirectory.CombineFile("App_Config", "Include", ".Sitecore.Settings.config"),
                    loggingService),
                new MoveDirectory(sitecoreDirectory.CombineDirectory("sitecore modules", "shell", "ucommerce", "install", "binaries"),
                    sitecoreDirectory.CombineDirectory("bin", "uCommerce"),
                    overwriteTarget: true,
                    loggingService),
                new DeleteFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Installer.dll"), loggingService),
                new DeleteFile(sitecoreDirectory.CombineFile("bin", "Ucommerce.Transactions.Payments.dll"), loggingService),
                new DeleteDirectory(appsDirectory.CombineDirectory("ServiceStack"), loggingService),
                new DeleteRavenDB(appsDirectory, loggingService),
                new MoveDirectory(appsDirectory.CombineDirectory("ExchangeRateAPICurrencyConversion.disabled"),
                    appsDirectory.CombineDirectory("ExchangeRateAPICurrencyConversion"),
                    true,
                    loggingService),
                new DeleteDirectory(appsDirectory.CombineDirectory("Catalogs"), loggingService),
                new DeleteDirectory(appsDirectory.CombineDirectory("Catalogs.disabled"), loggingService),
                new EnableSitecoreCompatibilityApp(versionChecker, sitecoreDirectory, loggingService),
                new DeleteDirectory(appsDirectory.CombineDirectory("Widgets", "CatalogSearch"), loggingService),
                new DeleteDirectory(appsDirectory.CombineDirectory("Widgets", "CatalogSearch.disabled"), loggingService),
                new MoveDirectory(appsDirectory.CombineDirectory("Sanitization.disabled"),
                    appsDirectory.CombineDirectory("Sanitization"),
                    true,
                    loggingService),
                new DeleteFile(appsDirectory.CombineFile("Sanitization", "bin", "AngleSharp.dll"), loggingService),
                new DeleteFile(appsDirectory.CombineFile("Sanitization", "bin", "HtmlSanitizer.dll"), loggingService),
                new DeleteFile(sitecoreDirectory.CombineFile("Sanitization", "bin", "HtmlSanitizer.dll"), loggingService),
                new DeleteFile(sitecoreDirectory.CombineFile("sitecore modules",
                        "shell",
                        "ucommerce",
                        "Configuration",
                        "Payments.config"),
                    loggingService),
                new MoveUcommerceBinaries(sitecoreDirectory, loggingService),
                new MoveResourceFiles(sitecoreDirectory, connectionStringLocator, loggingService),
                new RenameConfigDefaultFilesToConfigFiles(sitecoreDirectory, loggingService),
                new MoveDirectoryIfTargetExist(appsDirectory.CombineDirectory("SimpleInventory.disabled"),
                    appsDirectory.CombineDirectory("SimpleInventory"),
                    loggingService),
                new MoveDirectoryIfTargetExist(appsDirectory.CombineDirectory("Acquire and Cancel Payments.disabled"),
                    appsDirectory.CombineDirectory("Acquire and Cancel Payments"),
                    loggingService),
                new UpgradeSearchProviders(sitecoreDirectory, loggingService),
                new SearchProviderCleanup(appsDirectory, loggingService),
                new RemoveRenamedPipelines(sitecoreDirectory, loggingService),
                new ComposeMoveSitecoreConfigIncludes(sitecoreDirectory, versionChecker, loggingService),
                new DeleteFile(appsDirectory.CombineFile("Ucommerce.Search.Lucene", "bin", "System.Collections.Immutable.dll"),
                    loggingService),
                new DeleteFile(appsDirectory.CombineFile("Ucommerce.Search.Lucene.disabled", "bin", "System.Collections.Immutable.dll"),
                    loggingService),
                new AddHeadlessToIgnoreUrlPrefixes(sitecoreDirectory, loggingService)
            });
        }
    }
}
