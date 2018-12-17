using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
    public class MoveUcommerceBinaries : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var _postInstallationSteps = new List<IPostStep>();

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            // Therefor any older version located in the bin folder needs to be removed.
            _postInstallationSteps.Add(new DeleteFile("~/bin/UCommerce.Sitecore.CommerceConnect.dll"));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Infrastructure.dll",
                "~/bin/UCommerce.Infrastructure.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Sitecore.dll",
                "~/bin/UCommerce.Sitecore.dll",
                backupTarget: false));

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Sitecore.CommerceConnect.dll",
                "~/sitecore modules/Shell/uCommerce/Apps/Sitecore Commerce Connect.disabled/UCommerce.Sitecore.CommerceConnect.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Web.Api.dll",
                "~/bin/UCommerce.Web.Api.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.SystemHttp.dll",
                "~/bin/UCommerce.SystemHttp.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.SystemWeb.dll",
                "~/bin/UCommerce.SystemWeb.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Web.Shell.dll",
                "~/bin/UCommerce.Web.Shell.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.dll",
                "~/bin/UCommerce.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Admin.dll",
                "~/bin/UCommerce.Admin.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Pipelines.dll",
                "~/bin/UCommerce.Pipelines.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Presentation.dll",
                "~/bin/UCommerce.Presentation.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.NHibernate.dll",
                "~/bin/UCommerce.NHibernate.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/UCommerce.Sitecore.Web.dll",
                "~/bin/UCommerce.Sitecore.Web.dll",
                backupTarget: false));

            foreach (var postInstallationStep in _postInstallationSteps)
            {
                postInstallationStep.Run(output, metaData);
            }
        }
    }
}
