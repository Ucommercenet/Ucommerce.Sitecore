using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.Steps;
using FileBackup = Ucommerce.Sitecore.Installer.Steps.FileBackup;

namespace Ucommerce.Sitecore.Installer
{
    public class PostInstallationStep : IPostStep
    {
        private readonly IList<IPostStep> _postInstallationSteps;

        /// <summary>
        ///     The uCommerce post installation step.
        /// </summary>
        /// <remarks>
        ///     There is a race condition between upgrading the database and upgrading the binaries. :-(
        ///     Upgrade the database first, and the old binaries might not work with the new database.
        ///     Upgrade the binaries first, and the new binaries might not work with the old database.
        ///     We have one observation indicating a failed installation because the new binaries was
        ///     activated before the database scripts were done, resulting in a broken system.
        ///     The problem is probably going to grow, as more database migrations are added.
        ///     We have chosen to upgrade the database first.
        ///     This is because the database upgrade takes a long time in the clean scenario, but is
        ///     relatively faster in upgrade scenarios.
        ///     So for clean installs there are no old binaries, so the race condition is void.
        ///     - Jesper
        /// </remarks>
        public PostInstallationStep()
        {
            var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();
            IDatabaseAvailabilityService sitefinityDatabaseAvailabilityService =
                new SitecoreDatabaseAvailabilityService();
            var installationConnectionStringLocator = new SitecoreInstallationConnectionStringLocator("");
            var runtimeVersionChecker =
                new RuntimeVersionChecker(installationConnectionStringLocator, sitecoreInstallerLoggingService);
            var updateService = new UpdateService(installationConnectionStringLocator, runtimeVersionChecker,
                sitefinityDatabaseAvailabilityService);
            var sitecoreVersionChecker = new SitecoreVersionChecker();
            var virtualAppsPath = "~/sitecore modules/Shell/uCommerce/Apps";

            _postInstallationSteps = new List<IPostStep>();

            _postInstallationSteps.Add(new SitecorePreRequisitesChecker());
            _postInstallationSteps.Add(new InitializeObjectFactory());
            _postInstallationSteps.Add(new InstallDatabase("~/sitecore modules/Shell/ucommerce/install"));
            _postInstallationSteps.Add(new InstallDatabaseSitecore("~/sitecore modules/Shell/ucommerce/install"));
            _postInstallationSteps.Add(new UpdateUCommerceAssemblyVersionInDatabase(updateService,
                runtimeVersionChecker, sitecoreInstallerLoggingService));

            _postInstallationSteps.Add(new CopyFile("~/web.config",
                "~/web.config.{DateTime.Now.Ticks}.backup"));
            _postInstallationSteps.Add(new SitecoreWebconfigMerger(sitecoreVersionChecker));
            _postInstallationSteps.Add(new SeperateConfigSectionInNewFile("configuration/sitecore/settings",
                "~/web.config", "~/App_Config/Include/.Sitecore.Settings.config"));
            _postInstallationSteps.Add(new MoveDirectory("~/sitecore modules/shell/ucommerce/install/binaries",
                "~/bin/uCommerce", true));

            _postInstallationSteps.Add(new DeleteFile("~/bin/ucommerce/Ucommerce.Installer.dll"));

            // Remove old UCommerce.Transactions.Payment.dll from /bin since payment methods have been moved to Apps.
            _postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Transactions.Payments.dll"));
            // Remove ServiceStack folder
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/ServiceStack"));

            // Remove RavenDB apps (in V9 Raven has been replaced by Lucene)
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB25"));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}RavenDB25.disabled"));
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB30"));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}RavenDB30.disabled"));
            // Enable ExchangeRateAPICurrencyConversion app
            _postInstallationSteps.Add(new MoveDirectory(
                $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion.disabled",
                $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion", true));

            // Remove Catalogs app since it was moved into Core
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/Catalogs"));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Catalogs.disabled"));
            _postInstallationSteps.Add(
                new EnableSitecoreCompatibilityApp(sitecoreVersionChecker, sitecoreInstallerLoggingService));

            // Remove CatalogSearch widget
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch"));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch.disabled"));

            // Enable Sanitization app
            _postInstallationSteps.Add(new MoveDirectory($"{virtualAppsPath}/Sanitization.disabled",
                $"{virtualAppsPath}/Sanitization", true));
            _postInstallationSteps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/AngleSharp.dll"));
            _postInstallationSteps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/HtmlSanitizer.dll"));

            //Clean up unused configuration since payment integration has move to apps
            _postInstallationSteps.Add(
                new DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config"));

            _postInstallationSteps.Add(new MoveUcommerceBinaries());
           // _postInstallationSteps.Add(new MoveResourceFiles());

            ComposeConfiguration();
            ComposePipelineConfiguration();
            _postInstallationSteps.Add(
                new RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Apps", false));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist(
                $"{virtualAppsPath}/SimpleInventory.disabled",
                $"{virtualAppsPath}/SimpleInventory"));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist(
                $"{virtualAppsPath}/Acquire and Cancel Payments.disabled",
                $"{virtualAppsPath}/Acquire and Cancel Payments"));
            // Set up search providers
            ToggleActiveSearchProvider(virtualAppsPath);

            // Clean lucene from disk
            SearchProviderCleanup("~/sitecore modules/Shell/uCommerce/Apps");

            //Create back up and remove old files
            RemovedRenamedPipelines();

            _postInstallationSteps.Add(new CreateApplicationShortcuts());
            _postInstallationSteps.Add(new CreateSpeakApplicationIfSupported(sitecoreVersionChecker));

            // Move sitecore config includes into the right path
            ComposeMoveSitecoreConfigIncludes(sitecoreVersionChecker);

            // Clean up System.Collections.Immutable.dll in Lucene App since it is no longer used
            _postInstallationSteps.Add(
                new DeleteFile($"{virtualAppsPath}/Ucommerce.Search.Lucene/bin/System.Collections.Immutable.dll"));
            _postInstallationSteps.Add(new DeleteFile(
                $"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled/bin/System.Collections.Immutable.dll"));
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            foreach (var step in _postInstallationSteps)
            {
                IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

                try
                {
                    step.Run(output, metaData);
                    logging.Information<PostInstallationStep>($"Executed: {step.GetType().FullName}");
                }
                catch (Exception ex)
                {
                    logging.Error<PostInstallationStep>(ex, step.GetType().FullName);

                    throw;
                }
            }
        }

        private void SearchProviderCleanup(string virtualAppsPath)
        {
            var luceneIndexesFolderPath =
                HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene/Configuration/Indexes");
            var luceneIndexesFolderPathDisabled =
                HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled/Configuration/Indexes");

            if (Directory.Exists(luceneIndexesFolderPath)) Directory.Delete(luceneIndexesFolderPath, true);

            if (Directory.Exists(luceneIndexesFolderPathDisabled))
                Directory.Delete(luceneIndexesFolderPathDisabled, true);
        }

        private void ToggleActiveSearchProvider(string virtualAppsPath)
        {
            var luceneAppFolderPath = HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene");
            var luceneAppDisaledFolderPath =
                HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled");
            var elasticAppFolderPath = HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.ElasticSearch");
            var elasticAppDisabledFolderPath =
                HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.ElasticSearch.disabled");

            // If Elastic is enabled, replace the app, and make sure Lucene is then disabled.
            if (Directory.Exists(elasticAppFolderPath))
            {
                new DirectoryMoverIfTargetExist(
                        new DirectoryInfo(elasticAppDisabledFolderPath),
                        new DirectoryInfo(elasticAppFolderPath))
                    .Move(null);

                new DirectoryMover(
                        new DirectoryInfo(luceneAppFolderPath),
                        new DirectoryInfo(luceneAppDisaledFolderPath), true)
                    .Move(null);
            }
        }

        private void ComposePipelineConfiguration()
        {
            _postInstallationSteps.Add(new RenameConfigDefaultFilesToConfigFilesStep(
                "~/sitecore modules/Shell/uCommerce/Pipelines", false));
        }

        private void ComposeConfiguration()
        {
            _postInstallationSteps.Add(new RenameConfigDefaultFilesToConfigFilesStep(
                "~/sitecore modules/Shell/uCommerce/Configuration", false
            ));
        }

        private void RemovedRenamedPipelines()
        {
            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Basket.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Basket.config"
            ));


            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Checkout.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Checkout.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCampaignItem.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCampaignItem.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCategory.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCategory.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDataType.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDataType.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDefinition.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDefinition.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteLanguage.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteLanguage.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProduct.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProduct.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalog.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalog.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalogGroup.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalogGroup.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductDefinitionField.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductDefinitionField.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Processing.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Processing.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReview.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReview.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReviewComment.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReviewComment.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveCategory.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveCategory.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDataType.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDataType.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinition.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinition.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinitionField.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinitionField.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveLanguage.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveLanguage.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveOrder.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveOrder.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProduct.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProduct.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalog.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalog.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalogGroup.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalogGroup.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductDefinitionField.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductDefinitionField.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCancelled.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCancelled.config"
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCompletedOrder.config"
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCompletedOrder.config"
            ));
        }

        private void ComposeMoveSitecoreConfigIncludes(SitecoreVersionChecker versionChecker)
        {
            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Databases.config",
                "~/App_Config/include/Sitecore.uCommerce.Databases.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Dataproviders.config",
                "~/App_Config/include/Sitecore.uCommerce.Dataproviders.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Events.config",
                "~/App_Config/include/Sitecore.uCommerce.Events.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Sites.config",
                "~/App_Config/include/Sitecore.uCommerce.Sites.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                true));

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    true));
            else
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    true));

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 1)))
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.9.1.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    true));
            else
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Settings.config",
                "~/App_Config/include/Sitecore.uCommerce.Settings.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                true));

            _postInstallationSteps.Add(new MoveFileIfTargetExist(
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config",
                true));

            if (versionChecker.IsLowerThan(new Version(8, 2)))
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.WebApiConfiguration.config.disabled",
                    "~/App_Config/include/Sitecore.uCommerce.WebApiConfiguration.config",
                    true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.initialize.config",
                "~/App_Config/include/Sitecore.uCommerce.initialize.config",
                true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Log4net.config",
                "~/App_Config/include/Sitecore.uCommerce.Log4net.config",
                true));
        }
    }
}