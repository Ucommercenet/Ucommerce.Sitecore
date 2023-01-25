using System.IO;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using J2N.Collections.Generic;
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
                new CopyFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config")),
                    new FileInfo(Path.Combine(sitecoreDirectory.FullName, "web.config.{DateTime.Now.Ticks}.backup")),
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
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "ServiceStack")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB25")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB25.disabled")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB30")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "RavenDB30.disabled")), loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath, "ExchangeRateAPICurrencyConversion.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath, "ExchangeRateAPICurrencyConversion")),
                    true,
                    loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Catalogs")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Catalogs.disabled")), loggingService),
                new EnableSitecoreCompatibilityApp(versionChecker, sitecoreDirectory, loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Widgets", "CatalogSearch")), loggingService),
                new DeleteDirectory(new DirectoryInfo(Path.Combine(appsPath, "Widgets", "CatalogSearch.disabled")), loggingService),
                new MoveDirectory(new DirectoryInfo(Path.Combine(appsPath, "Sanitization.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath, "Sanitization")),
                    true,
                    loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath, "Sanitization", "bin", "AngleSharp.dll")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(appsPath, "Sanitization", "bin", "HtmlSanitizer.dll")), loggingService),
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
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Configuration")),
                    false,
                    loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Pipelines")),
                    false,
                    loggingService),
                new RenameConfigDefaultFilesToConfigFilesStep(
                    new DirectoryInfo(Path.Combine(sitecoreDirectory.FullName, "sitecore modules", "Shell", "uCommerce", "Apps")),
                    false,
                    loggingService),
                new MoveDirectoryIfTargetExist(new DirectoryInfo(Path.Combine(appsPath, "SimpleInventory.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath, "SimpleInventory")),
                    loggingService),
                new MoveDirectoryIfTargetExist(new DirectoryInfo(Path.Combine(appsPath, "Acquire and Cancel Payments.disabled")),
                    new DirectoryInfo(Path.Combine(appsPath, "Acquire and Cancel Payments")),
                    loggingService),
            });
            Steps.AddRange(ToggleActiveSearchProviderSteps(appsPath, loggingService));
        }

        private List<IStep> ToggleActiveSearchProviderSteps(string appsPath, IInstallerLoggingService loggingService)
        {
            var luceneAppFolderPath = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.Lucene"));
            var luceneAppDisabledFolderPath = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.Lucene.disabled"));
            var elasticAppFolderPath = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.ElasticSearch"));
            var elasticAppDisabledFolderPath = new DirectoryInfo(Path.Combine(appsPath, "Ucommerce.Search.ElasticSearch.disabled"));
            var steps = new List<IStep> { };
            if (Directory.Exists(elasticAppFolderPath.FullName))
            {
                steps.Add(new MoveDirectoryIfTargetExist(elasticAppDisabledFolderPath, elasticAppFolderPath, loggingService));
                steps.Add(new MoveDirectoryIfTargetExist(luceneAppFolderPath, luceneAppDisabledFolderPath, loggingService));
            }

            return steps;
        }

        private List<IStep> RemoveRenamedPipelinesSteps(DirectoryInfo sitecoreDirectory, IInstallerLoggingService loggingService)
        {
            var pipelinesPath = Path.Combine("sitecore modules", "Shell", "ucommerce", "Pipelines");

            var steps = new List<IStep>
            {
                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Basket.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Basket.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Checkout.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Checkout.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteCampaignItem.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteCampaignItem.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteCategory.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteCategory.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteDataType.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteDataType.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteDefinition.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteDefinition.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteLanguage.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteLanguage.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProduct.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProduct.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductCatalog.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductCatalog.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductCatalogGroup.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductCatalogGroup.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductDefinitionField.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "DeleteProductDefinitionField.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Processing.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "Processing.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ProductReview.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ProductReview.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ProductReviewComment.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ProductReviewComment.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveCategory.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveCategory.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDataType.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDataType.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDefinition.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDefinition.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDefinitionField.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveDefinitionField.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveLanguage.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveLanguage.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveOrder.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveOrder.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProduct.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProduct.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductCatalog.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductCatalog.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductCatalogGroup.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductCatalogGroup.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductDefinitionField.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "SaveProductDefinitionField.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ToCancelled.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ToCancelled.config")), loggingService),

                new FileBackup(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ToCompletedOrder.config")), loggingService),
                new DeleteFile(new FileInfo(Path.Combine(sitecoreDirectory.FullName, pipelinesPath, "ToCompletedOrder.config")), loggingService),
            };
            return steps;
        }

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
