using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.Steps;
using DeleteFile = Ucommerce.Sitecore.Installer.Steps.DeleteFile;
using FileBackup = Ucommerce.Sitecore.Installer.Steps.FileBackup;
using UpdateUCommerceAssemblyVersionInDatabase =
    Ucommerce.Sitecore.Installer.Steps.UpdateUCommerceAssemblyVersionInDatabase;

namespace Ucommerce.Sitecore.Installer
{
    public class PostInstallationStep : IPostStep
    {
        private readonly IInstallerLoggingService _logging = new SitecoreInstallerLoggingService();
        private readonly IList<IPostStep> _postInstallationSteps;

        /// <summary>
        /// The uCommerce post installation step.
        /// </summary>
        /// <remarks>
        /// There is a race condition between upgrading the database and upgrading the binaries. :-(
        ///
        /// Upgrade the database first, and the old binaries might not work with the new database.
        /// Upgrade the binaries first, and the new binaries might not work with the old database.
        ///
        /// We have one observation indicating a failed installation because the new binaries was
        /// activated before the database scripts were done, resulting in a broken system.
        ///
        /// The problem is probably going to grow, as more database migrations are added.
        ///
        /// We have chosen to upgrade the database first.
        /// This is because the database upgrade takes a long time in the clean scenario, but is
        /// relatively faster in upgrade scenarios.
        ///
        /// So for clean installs there are no old binaries, so the race condition is void.
        /// - Jesper
        /// </remarks>
        public PostInstallationStep()
        {
            IDatabaseAvailabilityService sitefinityDatabaseAvailabilityService =
                new SitecoreDatabaseAvailabilityService();
            var installationConnectionStringLocator = new SitecoreInstallationConnectionStringLocator();
            var runtimeVersionChecker =
                new RuntimeVersionChecker(installationConnectionStringLocator, _logging);
            var updateService = new UpdateService(installationConnectionStringLocator, runtimeVersionChecker,
                sitefinityDatabaseAvailabilityService);
            var sitecoreVersionChecker = new SitecoreVersionChecker();
            var virtualAppsPath = "~/sitecore modules/Shell/uCommerce/Apps";

            _postInstallationSteps = new List<IPostStep>();

            _postInstallationSteps.Add(new SitecorePreRequisitesChecker(_logging));
            _postInstallationSteps.Add(new InitializeObjectFactory());
            _postInstallationSteps.Add(new InstallDatabase("~/sitecore modules/Shell/ucommerce/install", _logging));
            _postInstallationSteps.Add(new InstallDatabaseSitecore("~/sitecore modules/Shell/ucommerce/install", _logging));
            _postInstallationSteps.Add(new UpdateUCommerceAssemblyVersionInDatabase(updateService,
                runtimeVersionChecker, _logging));

            _postInstallationSteps.Add(new CopyFile("~/web.config",
                targetVirtualPath: "~/web.config.{DateTime.Now.Ticks}.backup", _logging));
            _postInstallationSteps.Add(new SitecoreWebconfigMerger(_logging));
            _postInstallationSteps.Add(new SeperateConfigSectionInNewFile("configuration/sitecore/settings",
                "~/web.config", "~/App_Config/Include/.Sitecore.Settings.config", _logging));
            _postInstallationSteps.Add(new MoveDirectory("~/sitecore modules/shell/ucommerce/install/binaries",
                "~/bin/uCommerce", overwriteTarget: true, _logging));

            _postInstallationSteps.Add(new DeleteFile("~/bin/ucommerce/Ucommerce.Installer.dll", _logging));

            // Remove old UCommerce.Transactions.Payment.dll from /bin since payment methods have been moved to Apps.
            _postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Transactions.Payments.dll", _logging));
            // Remove ServiceStack folder
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/ServiceStack", _logging));

            // Remove RavenDB apps (in V9 Raven has been replaced by Lucene)
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB25", _logging));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}RavenDB25.disabled", _logging));
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/RavenDB30", _logging));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}RavenDB30.disabled", _logging));
            // Enable ExchangeRateAPICurrencyConversion app
            _postInstallationSteps.Add(new MoveDirectory(
                $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion.disabled",
                $"{virtualAppsPath}/ExchangeRateAPICurrencyConversion", true, _logging));

            // Remove Catalogs app since it was moved into Core
            _postInstallationSteps.Add(new DeleteDirectory($"{virtualAppsPath}/Catalogs", _logging));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Catalogs.disabled", _logging));
            _postInstallationSteps.Add(
                new EnableSitecoreCompatibilityApp(sitecoreVersionChecker, _logging));

            // Remove CatalogSearch widget
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch", _logging));
            _postInstallationSteps.Add(
                new DeleteDirectory($"{virtualAppsPath}/Widgets/CatalogSearch.disabled", _logging));

            // Enable Sanitization app
            _postInstallationSteps.Add(new MoveDirectory($"{virtualAppsPath}/Sanitization.disabled", $"{virtualAppsPath}/Sanitization", true, _logging));
            _postInstallationSteps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/AngleSharp.dll", _logging));
            _postInstallationSteps.Add(new DeleteFile($"{virtualAppsPath}/Sanitization/bin/AngleSharp.dll", _logging));

            //Clean up unused configuration since payment integration has move to apps
            _postInstallationSteps.Add(
                new DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config", _logging));

            _postInstallationSteps.Add(new MoveUcommerceBinaries(_logging));
            _postInstallationSteps.Add(new MoveResourceFiles(_logging));

            ComposeConfiguration();
            ComposePipelineConfiguration();
            _postInstallationSteps.Add(
                new RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Apps", false, _logging));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist(
                $"{virtualAppsPath}/SimpleInventory.disabled",
                $"{virtualAppsPath}/SimpleInventory", _logging));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist(
                $"{virtualAppsPath}/Acquire and Cancel Payments.disabled",
                $"{virtualAppsPath}/Acquire and Cancel Payments", _logging));
            SearchProviderCleanup(virtualAppsPath);
            ToggleActiveSearchProvider(virtualAppsPath);
            //Create back up and remove old files
            RemovedRenamedPipelines();

            _postInstallationSteps.Add(new CreateApplicationShortcuts());
            _postInstallationSteps.Add(new CreateSpeakApplicationIfSupported(sitecoreVersionChecker, _logging));

            // Move sitecore config includes into the right path
            ComposeMoveSitecoreConfigIncludes(sitecoreVersionChecker);
        }

        private void SearchProviderCleanup(string virtualAppsPath)
        {
            var luceneIndexesFolderPath = HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene/Configuration/Indexes");
            var luceneIndexesFolderPathDisabled = HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled/Configuration/Indexes");

            if (Directory.Exists(luceneIndexesFolderPath))
            {
                Directory.Delete(luceneIndexesFolderPath, true);
            }

            if (Directory.Exists(luceneIndexesFolderPathDisabled))
            {
                Directory.Delete(luceneIndexesFolderPathDisabled, true);
            }
        }

        private void ToggleActiveSearchProvider(string virtualAppsPath)
        {
            // If Elastic is enabled, replace the app, and make sure Lucene is then disabled.
            if (Directory.Exists(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.ElasticSearch")))
            {
                new DirectoryMoverIfTargetExist(
                        new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.ElasticSearch.disabled")),
                        new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.ElasticSearch")))
                    .Move(null);

                new DirectoryMover(
                        new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene")),
                        new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled")), true)
                    .Move(null);
            }

            new DirectoryMoverIfTargetExist(
                    new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene")),
                    new DirectoryInfo(HostingEnvironment.MapPath($"{virtualAppsPath}/Ucommerce.Search.Lucene.disabled")))
                .Move(null);
        }

        private void ComposePipelineConfiguration()
        {
            _postInstallationSteps.Add(new RenameConfigDefaultFilesToConfigFilesStep(
                "~/sitecore modules/Shell/uCommerce/Pipelines", false, _logging));
        }

        private void ComposeConfiguration()
        {
            _postInstallationSteps.Add(new RenameConfigDefaultFilesToConfigFilesStep(
                "~/sitecore modules/Shell/uCommerce/Configuration", false, _logging
            ));
        }

        private void RemovedRenamedPipelines()
        {
            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Basket.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Basket.config", _logging
            ));


            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Checkout.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Checkout.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCampaignItem.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCampaignItem.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCategory.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCategory.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDataType.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDataType.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDefinition.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDefinition.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteLanguage.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteLanguage.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProduct.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProduct.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalog.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalog.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalogGroup.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalogGroup.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Processing.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/Processing.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReview.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReview.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReviewComment.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ProductReviewComment.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveCategory.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveCategory.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDataType.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDataType.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinition.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinition.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveLanguage.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveLanguage.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveOrder.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveOrder.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProduct.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProduct.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalog.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalog.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalogGroup.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalogGroup.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductDefinitionField.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCancelled.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCancelled.config", _logging
            ));

            _postInstallationSteps.Add(new FileBackup(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCompletedOrder.config", _logging
            ));

            _postInstallationSteps.Add(new DeleteFile(
                "~/sitecore modules/Shell/ucommerce/Pipelines/ToCompletedOrder.config", _logging
            ));
        }

        private void ComposeMoveSitecoreConfigIncludes(SitecoreVersionChecker versionChecker)
        {
            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Databases.config",
                "~/App_Config/include/Sitecore.uCommerce.Databases.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Dataproviders.config",
                "~/App_Config/include/Sitecore.uCommerce.Dataproviders.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Events.config",
                "~/App_Config/include/Sitecore.uCommerce.Events.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Sites.config",
                "~/App_Config/include/Sitecore.uCommerce.Sites.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                backupTarget: true, _logging));

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 3)))
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.9.3.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    backupTarget: true, _logging));
            }
            else
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                    backupTarget: true, _logging));
            }

            if (versionChecker.IsEqualOrGreaterThan(new Version(9, 1)))
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.9.1.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    backupTarget: true, _logging));
            }
            else
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                    backupTarget: true, _logging));
            }

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Settings.config",
                "~/App_Config/include/Sitecore.uCommerce.Settings.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFileIfTargetExist(
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config",
                backupTarget: true, _logging));

            if (versionChecker.IsLowerThan(new Version(8, 2)))
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.WebApiConfiguration.config.disabled",
                    "~/App_Config/include/Sitecore.uCommerce.WebApiConfiguration.config",
                    backupTarget: true, _logging));
            }

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.initialize.config",
                "~/App_Config/include/Sitecore.uCommerce.initialize.config",
                backupTarget: true, _logging));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Log4net.config",
                "~/App_Config/include/Sitecore.uCommerce.Log4net.config",
                backupTarget: true, _logging));
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            foreach (var step in _postInstallationSteps)
            {
                try
                {
                    step.Run(output, metaData);
                    _logging.Information<PostInstallationStep>("Executed: {FullName}", step.GetType().FullName);
                }
                catch (Exception ex)
                {
                    _logging.Error<PostInstallationStep>(ex, "Failed to execute step: {FullName}", step.GetType().FullName);

                    throw;
                }
            }
        }
    }
}