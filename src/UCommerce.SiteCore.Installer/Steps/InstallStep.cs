using System;
using System.IO;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
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
            var appsPath = new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Apps"));
            Steps.AddRange(new IStep[]
            {
                new SitecorePreRequisitesChecker(connectionStringLocator, loggingService),
                new InitializeObjectFactory(loggingService),
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, loggingService),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, loggingService),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, loggingService),
                new CopyDirectory(new DirectoryInfo(Path.Combine(baseDirectory.FullName, "package", "files")), sitecoreDirectory, true, loggingService),
                new CopyFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, $"web.config.{DateTime.Now.Ticks}.backup")),
                    loggingService),
                new SitecoreWebconfigMerger(sitecoreDirectory, loggingService),
                new SeperateConfigSectionInNewFile("configuration/sitecore/settings",
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, "App_Config", "Include", ".Sitecore.Settings.config")),
                    loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "shell", "ucommerce", "install", "binaries")),
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "bin", "uCommerce")),
                    overwriteTarget: true,
                    loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "bin", "ucommerce", "Ucommerce.Installer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "bin", "Ucommerce.Transactions.Payments.dll")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "ServiceStack")), loggingService),
                new DeleteRavenDB(appsPath, loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "ExchangeRateAPICurrencyConversion.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath.FullName, "ExchangeRateAPICurrencyConversion")),
                    true,
                    loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "Catalogs")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "Catalogs.disabled")), loggingService),
                new EnableSitecoreCompatibilityApp(versionChecker, sitecoreDirectory, loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "Widgets", "CatalogSearch")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "Widgets", "CatalogSearch.disabled")), loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath.FullName, "Sanitization.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath.FullName, "Sanitization")),
                    true,
                    loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath.FullName, "Sanitization", "bin", "AngleSharp.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath.FullName, "Sanitization", "bin", "HtmlSanitizer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "Sanitization", "bin", "HtmlSanitizer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,
                        "sitecore modules",
                        "shell",
                        "ucommerce",
                        "Configuration",
                        "Payments.config")),
                    loggingService),
                new MoveUcommerceBinaries(baseDirectory, sitecoreDirectory, loggingService),
                new MoveResourceFiles(baseDirectory, connectionStringLocator, loggingService),
                new RenameConfigDefaultFilesToConfigFiles(sitecoreDirectory, loggingService),
                new MoveDirectoryIfTargetExist(new DirectoryInfo(Path.Combine(appsPath.FullName, "SimpleInventory.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath.FullName, "SimpleInventory")),
                    loggingService),
                new MoveDirectoryIfTargetExist(new DirectoryInfo(Path.Combine(appsPath.FullName, "Acquire and Cancel Payments.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath.FullName, "Acquire and Cancel Payments")),
                    loggingService),
                new UpgradeSearchProviders(sitecoreDirectory, loggingService),
                new SearchProviderCleanup(appsPath.FullName, loggingService),
                new RemoveRenamedPipelines(sitecoreDirectory, loggingService),
                new ComposeMoveSitecoreConfigIncludes(sitecoreDirectory, versionChecker, loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath.FullName, "Ucommerce.Search.Lucene", "bin", "System.Collections.Immutable.dll")),
                    loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath.FullName, "Ucommerce.Search.Lucene.disabled", "bin", "System.Collections.Immutable.dll")),
                    loggingService),
                new AddHeadlessToIgnoreUrlPrefixes(sitecoreDirectory, loggingService)
            });
        }
    }
}
