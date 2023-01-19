using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveUcommerceBinaries : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var _postInstallationSteps = new List<IPostStep>();

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            // Therefor any older version located in the bin folder needs to be removed.
            //_postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Sitecore.CommerceConnect.dll"));

/*
            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Infrastructure.dll",
                "~/bin/Ucommerce.Infrastructure.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Sitecore.dll",
                "~/bin/Ucommerce.Sitecore.dll",
                backupTarget: false));

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Sitecore.CommerceConnect.dll",
                "~/sitecore modules/Shell/uCommerce/Apps/Sitecore Commerce Connect.disabled/Ucommerce.Sitecore.CommerceConnect.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Web.Api.dll",
                "~/bin/Ucommerce.Web.Api.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.SystemHttp.dll",
                "~/bin/Ucommerce.SystemHttp.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.SystemWeb.dll",
                "~/bin/Ucommerce.SystemWeb.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Web.Shell.dll",
                "~/bin/Ucommerce.Web.Shell.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.dll",
                "~/bin/Ucommerce.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Admin.dll",
                "~/bin/Ucommerce.Admin.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Pipelines.dll",
                "~/bin/Ucommerce.Pipelines.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Presentation.dll",
                "~/bin/Ucommerce.Presentation.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.NHibernate.dll",
                "~/bin/Ucommerce.NHibernate.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Sitecore.Web.dll",
                "~/bin/Ucommerce.Sitecore.Web.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Api.dll",
                "~/bin/Ucommerce.Api.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.Search.dll",
                "~/bin/Ucommerce.Search.dll",
                backupTarget: false));

            _postInstallationSteps.Add(new MoveFile(
                "~/bin/ucommerce/Ucommerce.SqlMultiReaderConnector.dll",
                "~/bin/Ucommerce.SqlMultiReaderConnector.dll",
                backupTarget: false));
            */
            foreach (var postInstallationStep in _postInstallationSteps)
            {
                postInstallationStep.Run(output, metaData);
            }
        }
    }
}
