using System.IO;
using Ucommerce.Infrastructure.Configuration;
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
            var appsPath = Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Apps");
            Steps.AddRange(new IStep[]
            {
                new PreRequisitesChecker(connectionStringLocator, loggingService),
                new InitializeObjectFactory(loggingService),
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, loggingService),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, loggingService),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, loggingService),
                new CopyFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config")), new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config.{DateTime.Now.Ticks}.backup")), loggingService),
                new SitecoreWebconfigMerger(sitecoreDirectory,loggingService),
                new SeperateConfigSectionInNewFile("configuration/sitecore/settings",new FileInfo(Path.Combine(sitecoreDirectory.FullName,"web.config")),new FileInfo(Path.Combine(sitecoreDirectory.FullName,"App_Config","Include",".Sitecore.Settings.config")),loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName,"sitecore modules","shell","ucommerce","install","binaries")),new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName,"bin","uCommerce")),overwriteTarget:true,loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,"bin","ucommerce","Ucommerce.Installer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName,"bin","Ucommerce.Transactions.Payments.dll")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath,"ServiceStack")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath,"RavenDB25")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath,"RavenDB25.disabled")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB30")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB30.disabled")), loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath, "ExchangeRateAPICurrencyConversion.disabled")), new DirectoryInfo(Path.Combine(appsPath, "ExchangeRateAPICurrencyConversion")), true, loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Catalogs")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Catalogs.disabled")), loggingService),
                new EnableSitecoreCompatibilityApp(versionChecker, sitecoreDirectory, loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Widgets", "CatalogSearch")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Widgets", "CatalogSearch.disabled")), loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath, "Sanitization.disabled")),new DirectoryInfo(Path.Combine(appsPath, "Sanitization")), true, loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath, "Sanitization", "bin", "AngleSharp.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath, "Sanitization", "bin", "HtmlSanitizer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "Sanitization", "bin", "HtmlSanitizer.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "shell","ucommerce","Configuration","Payments.config")), loggingService),
                new MoveUcommerceBinaries(baseDirectory,sitecoreDirectory,loggingService),
                new MoveResourceFiles(baseDirectory,connectionStringLocator,loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName,"sitecore modules","Shell","uCommerce","Configuration")),false,loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName,"sitecore modules","Shell","uCommerce","Pipelines")),false,loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName,"sitecore modules","Shell","uCommerce","Apps")),false,loggingService),
                new MoveDirectoryIfTargetExist( new DirectoryInfo(Path.Combine(appsPath, "SimpleInventory.disabled")), new DirectoryInfo(Path.Combine(appsPath, "SimpleInventory")), loggingService),
            });
        }
        
        //  steps.Add(new MoveDirectoryIfTargetExist(
        //      $"{virtualAppsPath}/SimpleInventory.disabled",
        //      $"{virtualAppsPath}/SimpleInventory"));
        //  steps.Add(new MoveDirectoryIfTargetExist(
        //      $"{virtualAppsPath}/Acquire and Cancel Payments.disabled",
        //      $"{virtualAppsPath}/Acquire and Cancel Payments"));
        // // Set up search providers
        //  ToggleActiveSearchProvider(virtualAppsPath);
        //
        //  // Clean lucene from disk
        //  SearchProviderCleanup("~/sitecore modules/Shell/uCommerce/Apps");
        //
        //  //Create back up and remove old files
        //  RemovedRenamedPipelines();
        //
        //  steps.Add(new CreateApplicationShortcuts());
        //  steps.Add(new CreateSpeakApplicationIfSupported(sitecoreVersionChecker));
        //
        //  // Move sitecore config includes into the right path
        //  ComposeMoveSitecoreConfigIncludes(sitecoreVersionChecker);
        //  
        //  // Clean up System.Collections.Immutable.dll in Lucene App since it is no longer used
        //  steps.Add(new DeleteFile($"{virtualAppsPath}/Ucommerce.Search.Lucene/bin/System.Collections.Immutable.dll"));
        //  steps.Add(new DeleteFile($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled/bin/System.Collections.Immutable.dll"));
        //
        //     return steps;
        // }
    }
}
