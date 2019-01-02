using System;
using System.Collections.Generic;
using UCommerce.Installer;
using UCommerce.Sitecore.Installer.Steps;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class MoveUcommerceBinaries : IInstallationStep
    {
        public void Execute()
        {
            var steps = new List<UCommerce.Installer.IInstallationStep>();

            steps.Add(new UCommerce.Installer.InstallerSteps.DeleteFile("~/bin/UCommerce.Sitecore.CommerceConnect.dll", new SitecoreInstallerLoggingService()));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Sitecore.dll", "~/bin/UCommerce.Sitecore.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Sitecore.CommerceConnect.dll", "~/bin/UCommerce.Sitecore.CommerceConnect.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Web.Api.dll", "~/bin/UCommerce.Web.Api.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.SystemHttp.dll", "~/bin/UCommerce.SystemHttp.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.SystemWeb.dll", "~/bin/UCommerce.SystemWeb.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Web.Shell.dll", "~/bin/UCommerce.Web.Shell.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Admin.dll", "~/bin/UCommerce.Admin.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.dll", "~/bin/UCommerce.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Pipelines.dll", "~/bin/UCommerce.Pipelines.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Presentation.dll", "~/bin/UCommerce.Presentation.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.NHibernate.dll", "~/bin/UCommerce.NHibernate.dll", false));
            steps.Add(new UCommerce.Sitecore.Installer.InstallationSteps.MoveFile("~/bin/ucommerce/UCommerce.Sitecore.Web.dll", "~/bin/UCommerce.Sitecore.Web.dll", false));
            
            foreach (var installationStep in steps)
            {
                installationStep.Execute();
            }
        }
    }
}