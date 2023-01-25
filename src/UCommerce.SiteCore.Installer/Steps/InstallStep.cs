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
            Steps.AddRange(new IStep[]
            {
                new PreRequisitesChecker(connectionStringLocator, loggingService),
                new InitializeObjectFactory(loggingService),
                new InstallDatabaseUcommerce(baseDirectory, connectionStringLocator, loggingService),
                new InstallDatabaseSitecore(baseDirectory, connectionStringLocator, loggingService),
                new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, loggingService),
                new CopyFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config")), new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config.{DateTime.Now.Ticks}.backup")), loggingService),
                new SitecoreWebconfigMerger(sitecoreDirectory,loggingService),
                new SeperateConfigSectionInNewFile("configuration/sitecore/settings",new FileInfo(Path.Combine(sitecoreDirectory.FullName,"web.config")),new FileInfo(Path.Combine(sitecoreDirectory.FullName,"/App_Config/Include/.Sitecore.Settings.config")),loggingService)
            });
        }
        //  steps.Add(new MoveDirectory("~/sitecore modules/shell/ucommerce/install/binaries",
        //      "~/bin/uCommerce", overwriteTarget: true));
        //
        //  steps.Add(new DeleteFile("~/bin/ucommerce/Ucommerce.Installer.dll"));
        //
        //  // Remove old UCommerce.Transactions.Payment.dll from /bin since payment methods have been moved to Apps.
        //  steps.Add(new DeleteFile("~/bin/Ucommerce.Transactions.Payments.dll"));
        //  // Remove ServiceStack folder
        //  steps.Add(new DeleteDirectory($"{virtualAppsPath}/ServiceStack"));
        //
        //  // Remove RavenDB apps (in V9 Raven has been replaced by Lucene)
        //  steps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB25"));
        //  steps.Add(
        //      new DeleteDirectory($"{virtualAppsPath}RavenDB25.disabled"));
        //  steps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB30"));
        //  steps.Add(
        //      new DeleteDirectory($"{virtualAppsPath}RavenDB30.disabled"));
        //  // Enable ExchangeRateAPICurrencyConversion app
        //  steps.Add(new MoveDirectory(
        //      $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion.disabled",
        //      $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion", true));
        //
        //  // Remove Catalogs app since it was moved into Core
        //  steps.Add(new DeleteDirectory($"{virtualAppsPath}/Catalogs"));
        //  steps.Add(
        //      new DeleteDirectory($"{virtualAppsPath}/Catalogs.disabled"));
        //  steps.Add(
        //      new EnableSitecoreCompatibilityApp(sitecoreVersionChecker, sitecoreInstallerLoggingService));
        //
        //  // Remove CatalogSearch widget
        //  steps.Add(
        //      new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch"));
        //  steps.Add(
        //      new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch.disabled"));
        //
        //  // Enable Sanitization app
        //  steps.Add(new MoveDirectory($"{virtualAppsPath}/Sanitization.disabled", $"{virtualAppsPath}/Sanitization", true));
        //  steps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/AngleSharp.dll"));
        //  steps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/HtmlSanitizer.dll"));
        //
        //  //Clean up unused configuration since payment integration has move to apps
        //  steps.Add(
        //      new DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config"));
        //
        //  steps.Add(new MoveUcommerceBinaries());
        //  steps.Add(new MoveResourceFiles());
        //
        //  ComposeConfiguration();
        //  ComposePipelineConfiguration();
        //  steps.Add(
        //      new RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Apps", false));
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
