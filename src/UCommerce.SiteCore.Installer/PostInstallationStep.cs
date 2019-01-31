using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Installer;
using UCommerce.Installer.InstallerSteps;
using UCommerce.Sitecore.Installer.InstallationSteps;

namespace UCommerce.Sitecore.Installer
{
    public class PostInstallationStep : IPostStep
    {
        IList<IInstallationStep> _installationSteps = new List<IInstallationStep>();

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
            var installationConnectionStringLocator = new SitecoreInstallationConnectionStringLocator();
            var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();

            var runtimeVersionChecker = new RuntimeVersionChecker(installationConnectionStringLocator, sitecoreInstallerLoggingService);

            var pathToMigrations = new DirectoryInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install"));
            var updateService = new UpdateService(installationConnectionStringLocator, runtimeVersionChecker, new SitecoreDatabaseAvailabilityService());
            var sitecoreVersionChecker = new SitecoreVersionChecker();

            _installationSteps.Add(new SitecorePrerequisitesChecker());
            _installationSteps.Add(new InstallationSteps.InitializeObjectFactory());
            _installationSteps.Add(new DatabaseInstallerStep(new DbInstallerCore(installationConnectionStringLocator, new MigrationLoader().GetDatabaseMigrations(pathToMigrations), sitecoreInstallerLoggingService)));
            _installationSteps.Add(new InstallationSteps.InstallDatabaseSitecore("~/sitecore modules/Shell/ucommerce/install"));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.UpdateUCommerceAssemblyVersionInDatabase(updateService,runtimeVersionChecker, sitecoreInstallerLoggingService));
            _installationSteps.Add(new BackupFile("~/web.config", "~/web.config.{DateTime.Now.Ticks}.backup", sitecoreInstallerLoggingService));
            _installationSteps.Add(new InstallationSteps.SitecoreWebconfigMerger(sitecoreVersionChecker));
            _installationSteps.Add(new InstallationSteps.SeperateConfigSectionInNewFile("configuration/sitecore/settings", "~/web.config", "~/App_Config/Include/.Sitecore.Settings.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new InstallationSteps.MoveDirectory("~/sitecore modules/shell/ucommerce/install/binaries", "~/bin/uCommerce", overwriteTarget: true));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/bin/ucommerce/UCommerce.Installer.dll", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/bin/UCommerce.Transactions.Payments.dll", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteDirectory("~/sitecore modules/Shell/Ucommerce/Apps/ServiceStack", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveDirectory("~/sitecore modules/Shell/Ucommerce/Apps/ExchangeRateAPICurrencyConversion.disabled", "~/sitecore modules/Shell/Ucommerce/Apps/ExchangeRateAPICurrencyConversion", true));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveUcommerceBinaries());
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveResourceFiles());
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveResourceFiles());
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Configuration", false));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Pipelines", false));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.RenameConfigDefaultFilesToConfigFilesStep("~/sitecore modules/Shell/uCommerce/Apps", false));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveDirectoryIfTargetExist("~/sitecore modules/Shell/uCommerce/Apps/SimpleInventory.disabled", "~/sitecore modules/Shell/uCommerce/Apps/SimpleInventory"));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveDirectoryIfTargetExist("~/sitecore modules/Shell/uCommerce/Apps/Acquire and Cancel Payments.disabled", "~/sitecore modules/Shell/uCommerce/Apps/Acquire and Cancel Payments"));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveDirectoryIfTargetExist("~/sitecore modules/shell/uCommerce/Apps/RavenDB30.disabled", "~/sitecore modules/shell/uCommerce/Apps/RavenDB30"));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveDirectory("~/sitecore modules/shell/uCommerce/Apps/RavenDB25.disabled", "~/sitecore modules/shell/uCommerce/Apps/RavenDB25", true));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/Basket.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/Checkout.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCampaignItem.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteCategory.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDataType.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteDefinition.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteLanguage.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProduct.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalog.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductCatalogGroup.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/DeleteProductDefinitionField.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/Processing.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/ProductReview.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/ProductReviewComment.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveCategory.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveDataType.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinition.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveDefinitionField.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveLanguage.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveOrder.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveProduct.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalog.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductCatalogGroup.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/SaveProductDefinitionField.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/ToCancelled.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/sitecore modules/Shell/ucommerce/Pipelines/ToCompletedOrder.config", sitecoreInstallerLoggingService));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.CreateApplicationShortcuts());
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.CreateSpeakApplicationIfSupported(sitecoreVersionChecker));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Dataproviders.config", "~/App_Config/include/Sitecore.uCommerce.Dataproviders.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Events.config", "~/App_Config/include/Sitecore.uCommerce.Events.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.IdTable.config", "~/App_Config/include/Sitecore.uCommerce.IdTable.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Sites.config", "~/App_Config/include/Sitecore.uCommerce.Sites.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config", "~/App_Config/include/Sitecore.uCommerce.Pipelines.getItemPersonalizationVisibility.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config", "~/App_Config/include/Sitecore.uCommerce.Pipelines.HttpRequestBegin.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.PreProcessRequest.config", "~/App_Config/include/Sitecore.uCommerce.Pipelines.PreProcessRequest.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Settings.config", "~/App_Config/include/Sitecore.uCommerce.Settings.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled", "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFileIfTargetExist("~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config.disabled", "~/App_Config/include/Sitecore.uCommerce.Pipelines.ModifyPipelines.config", true));

            if (sitecoreVersionChecker.IsLowerThan(new Version(8, 2)))
            {
                _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.WebApiConfiguration.config.disabled", "~/App_Config/include/Sitecore.uCommerce.WebApiConfiguration.config", true));
            }

            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.initialize.config", "~/App_Config/include/Sitecore.uCommerce.initialize.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/sitecore modules/Shell/ucommerce/install/configInclude/Sitecore.uCommerce.Log4net.config", "~/App_Config/include/Sitecore.uCommerce.Log4net.config", true));
            _installationSteps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MigrateIdTableValues());
        }
        
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            IInstallerLoggingService logging = new SitecoreInstallerLoggingService();
            foreach (var step in _installationSteps)
            {
                try
                {
                    step.Execute();
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
