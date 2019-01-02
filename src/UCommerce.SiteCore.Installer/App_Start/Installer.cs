using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using UCommerce.Installer;
using UCommerce.Installer.InstallerSteps;
using UCommerce.Sitecore.Installer.InstallationSteps;

namespace UCommerce.Sitecore.Installer.App_Start
{
    public class Installer : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (new EventHandler(this.PreStart));
        }

        private static object _padLock = new object();
        private static bool _installationWasRun = false;

        public void PreStart(object sender, EventArgs e)
        {
            if (!_installationWasRun)
            {
                lock (_padLock)
                {
                    if (!_installationWasRun)
                    {
                        _installationWasRun = InstallInternal();
                    }
                }
            }
        }

        private bool InstallInternal()
        {
            var installationSteps = new List<IInstallationStep>();
            var sitecoreInstallerLoggingService = new SitecoreInstallerLoggingService();
            var installationConnectionStringLocator = new SitecoreInstallationConnectionStringLocator();
            var runtimeVersionChecker = new RuntimeVersionChecker(installationConnectionStringLocator, sitecoreInstallerLoggingService);
            var sitecoreDatabaseAvailabilityService = new SitecoreDatabaseAvailabilityService();
            var updateService = new UpdateService(installationConnectionStringLocator, runtimeVersionChecker, sitecoreDatabaseAvailabilityService);
            var sitecoreVersionChecker = new SitecoreVersionChecker();

            var pathToMigrations = new DirectoryInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install"));

            installationSteps.Add(new DatabaseInstallerStep(new DbInstallerCore(installationConnectionStringLocator, new MigrationLoader().GetDatabaseMigrations(pathToMigrations), sitecoreInstallerLoggingService)));
            //TODO: create sitecore database installer here
            installationSteps.Add(new UpdateUCommerceAssemblyVersionInDatabase(updateService, runtimeVersionChecker, sitecoreInstallerLoggingService));
            installationSteps.Add(new BackupFile("~/web.config", "~/web.config.{DateTime.Now.Ticks}.backup", sitecoreInstallerLoggingService));
            installationSteps.Add(new WebConfigTransformer("~/web.config", GetTransformations(sitecoreVersionChecker), sitecoreInstallerLoggingService));
            installationSteps.Add(new SeperateConfigSectionInNewFile("~/web.config", "~/App_Config/Include/.Sitecore.Settings.config",
                "configuration/sitecore/settings", sitecoreInstallerLoggingService));
            //move directory not needed as this is installing as nuget package
            installationSteps.Add(new DeleteFile("~/bin/ucommerce/UCommerce.Installer.dll", sitecoreInstallerLoggingService));
            installationSteps.Add(new DeleteFile("~/bin/UCommerce.Transactions.Payments.dll", sitecoreInstallerLoggingService));
            installationSteps.Add(new DeleteDirectory("~/sitecore modules/Shell/Ucommerce/Apps/ServiceStack", sitecoreInstallerLoggingService));
            installationSteps.Add(new UpdateUCommerceApps("~/sitecore modules/Shell/ucommerce/Apps", sitecoreInstallerLoggingService));
            installationSteps.Add(new DeleteFile("~/sitecore modules/shell/ucommerce/Configuration/Payments.config", sitecoreInstallerLoggingService));
            installationSteps.Add(new CreateApplicationShortcuts());
            installationSteps.Add(new MigrateIdTableValues());
            //exchange rate app always enabled through nuget.
            //move binaries and app global resources not needed.
            //compose configuration and pipeline not needed, already updated via package.


            var installer = new UCommerce.Installer.Installer(installationSteps, sitecoreInstallerLoggingService);
            installer.Execute();
            return false;
        }

        private IList<Transformation> GetTransformations(SitecoreVersionChecker sitecoreVersionChecker)
        {
            var transformations = new List<Transformation>()
            {
                new Transformation("~/sitecore modules/Shell/ucommerce/install/CleanConfig.config"),
                new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.config"),
                new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.IIS7.config", isIntegrated: true),
                new Transformation("~/sitecore modules/Shell/ucommerce/install/uCommerce.dependencies.sitecore.config"),
                new Transformation("~/sitecore modules/Shell/ucommerce/install/sitecore.config"),
                new Transformation("~/sitecore modules/Shell/ucommerce/install/ClientDependency.config")
            };

            if (sitecoreVersionChecker.IsLowerThan(new Version(8, 0)))
            {
                transformations.Add(
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/log4net.config"));
            }

            if (sitecoreVersionChecker.IsLowerThan(new Version(8, 1))) // Only add new assembly bindings for version 8.0 and earlier.
            {
                transformations.Add(
                    new Transformation("~/sitecore modules/Shell/ucommerce/install/updateAssemblyBinding.config"));
            }

            return transformations;
        }

        public void Dispose()
        {
           
        }
    }
}
