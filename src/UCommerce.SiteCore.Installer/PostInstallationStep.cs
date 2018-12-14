using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;
using UCommerce.Installer;
using UCommerce.Installer.InstallerSteps;
using UCommerce.Sitecore.Installer.Steps;
using DeleteFile = UCommerce.Sitecore.Installer.Steps.DeleteFile;
using FileBackup = UCommerce.Sitecore.Installer.Steps.FileBackup;
using UpdateUCommerceAssemblyVersionInDatabase = UCommerce.Sitecore.Installer.Steps.UpdateUCommerceAssemblyVersionInDatabase;

namespace UCommerce.Sitecore.Installer
{
    public class PostInstallationStep : IPostStep
    {
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
            var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();
            IDatabaseAvailabilityService sitefinityDatabaseAvailabilityService = new SitecoreDatabaseAvailabilityService();
            var installationConnectionStringLocator = new SitecoreInstallationConnectionStringLocator();
            var runtimeVersionChecker = new RuntimeVersionChecker(installationConnectionStringLocator, sitecoreInstallerLoggingService);
            var updateService = new UpdateService(installationConnectionStringLocator, runtimeVersionChecker, sitefinityDatabaseAvailabilityService);
            var sitecoreVersionChecker = new SitecoreVersionChecker();

            _postInstallationSteps = new List<IPostStep>();

            _postInstallationSteps.Add(new SitecorePreRequisitesChecker());
            _postInstallationSteps.Add(new InitializeObjectFactory());
            _postInstallationSteps.Add(new InstallDatabase("~/sitecore modules/Shell/ucommerce/install"));
            _postInstallationSteps.Add(new InstallDatabaseSitecore("~/sitecore modules/Shell/ucommerce/install"));
            _postInstallationSteps.Add(new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, sitecoreInstallerLoggingService));

            _postInstallationSteps.Add(new CopyFile(sourceVirtualPath: "~/web.config", targetVirtualPath: "~/web.config.{DateTime.Now.Ticks}.backup"));
            _postInstallationSteps.Add(new SitecoreWebconfigMerger(sitecoreVersionChecker));
            _postInstallationSteps.Add(new SeperateConfigSectionInNewFile("configuration/sitecore/settings", "~/web.config", "~/App_Config/Include/.Sitecore.Settings.config"));
            _postInstallationSteps.Add(new MoveDirectory("~/sitecore modules/shell/ucommerce/install/binaries", "~/bin/uCommerce", overwriteTarget: true));

            _postInstallationSteps.Add(new DeleteFile("~/bin/ucommerce/UCommerce.Installer.dll"));

            // Remove old UCommerce.Transactions.Payment.dll from /bin since payment methods have been moved to Apps.
            _postInstallationSteps.Add(new DeleteFile("~/bin/UCommerce.Transactions.Payments.dll"));
            // Remove ServiceStack folder
            _postInstallationSteps.Add(new UCommerce.Sitecore.Installer.Steps.DeleteDirectory("~/sitecore modules/Shell/Ucommerce/Apps/ServiceStack"));
            //Clean up unused configuration since payment integration has move to apps 
            _postInstallationSteps.Add(new DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config"));

            _postInstallationSteps.Add(new MoveUcommerceBinaries());
            _postInstallationSteps.Add(new MoveResourceFiles());

            ComposeConfiguration();
            ComposePipelineConfiguration();
            _postInstallationSteps.Add(new RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Apps", false));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist("~/sitecore modules/Shell/uCommerce/Apps/SimpleInventory.disabled", "~/sitecore modules/Shell/uCommerce/Apps/SimpleInventory"));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist("~/sitecore modules/Shell/uCommerce/Apps/Acquire and Cancel Payments.disabled", "~/sitecore modules/Shell/uCommerce/Apps/Acquire and Cancel Payments"));
            _postInstallationSteps.Add(new MoveDirectoryIfTargetExist("~/sitecore modules/shell/uCommerce/Apps/RavenDB30.disabled", "~/sitecore modules/shell/uCommerce/Apps/RavenDB30"));

            _postInstallationSteps.Add(new MoveDirectory("~/sitecore modules/shell/uCommerce/Apps/RavenDB25.disabled", "~/sitecore modules/shell/uCommerce/Apps/RavenDB25", true));
            //Create back up and remove old files
            RemovedRenamedPipelines();

            _postInstallationSteps.Add(new CreateApplicationShortcuts());
            _postInstallationSteps.Add(new CreateSpeakApplicationIfSupported(sitecoreVersionChecker));

            // Move sitecore config includes into the right path			
            ComposeMoveSitecoreConfigIncludes(sitecoreVersionChecker);

            _postInstallationSteps.Add(new MigrateIdTableValues());
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
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Dataproviders.config",
                "~/App_Config/include/Sitecore.uCommerce.Dataproviders.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Events.config",
                "~/App_Config/include/Sitecore.uCommerce.Events.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.IdTable.config",
                "~/App_Config/include/Sitecore.uCommerce.IdTable.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Sites.config",
                "~/App_Config/include/Sitecore.uCommerce.Sites.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Settings.config",
                "~/App_Config/include/Sitecore.uCommerce.Settings.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFileIfTargetExist(
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled",
                "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config",
                backupTarget: true));

            if (versionChecker.IsLowerThan(new Version(8, 2)))
            {
                _postInstallationSteps.Add(new MoveFile(
                    "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.WebApiConfiguration.config.disabled",
                    "~/App_Config/include/Sitecore.uCommerce.WebApiConfiguration.config",
                    backupTarget: true));
            }

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.initialize.config",
                "~/App_Config/include/Sitecore.uCommerce.initialize.config",
                backupTarget: true));

            _postInstallationSteps.Add(new MoveFile(
                "~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Log4net.config",
                "~/App_Config/include/Sitecore.uCommerce.Log4net.config",
                backupTarget: true));
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            foreach (var step in _postInstallationSteps)
            {
                IInstallerLoggingService logging = new SitecoreInstallerLoggingService();

                try
                {
                    step.Run(output, metaData);
                    logging.Log<PostInstallationStep>($"Executed: {step.GetType().FullName}");

                }
                catch (Exception ex)
                {
                    logging.Log<PostInstallationStep>(ex, step.GetType().FullName);

                    throw;
                }
            }
        }
    }
}
