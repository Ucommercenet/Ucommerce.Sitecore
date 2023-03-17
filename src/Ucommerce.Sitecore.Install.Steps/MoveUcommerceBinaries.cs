using System.IO;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Install.Steps.FileExtensions;

namespace Ucommerce.Sitecore.Install.Steps
{
    /// <summary>
    /// Aggregate step that moves Ucommerce binaries from the Ucommerce folder into bin
    /// </summary>
    public class MoveUcommerceBinaries : AggregateStep
    {
        private readonly IInstallerLoggingService _logging;

        public MoveUcommerceBinaries(DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _logging = logging;

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            // Therefor any older version located in the bin folder needs to be removed.
            //_postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Sitecore.CommerceConnect.dll"));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Infrastructure.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Infrastructure.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Sitecore.dll"),
                backupTarget: false,
                _logging));

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.CommerceConnect.dll"),
                sitecoreDirectory.CombineFile("sitecore modules",
                    "Shell",
                    "uCommerce",
                    "Apps",
                    "Sitecore Connect.disabled",
                    "Ucommerce.Sitecore.CommerceConnect.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Web.Api.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Web.Api.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SystemHttp.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.SystemHttp.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SystemWeb.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.SystemWeb.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Web.Shell.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Web.Shell.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Admin.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Admin.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Pipelines.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Pipelines.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Presentation.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Presentation.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.NHibernate.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.NHibernate.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Pipelines.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Pipelines.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.Web.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Sitecore.Web.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Api.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Api.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Search.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.Search.dll"),
                backupTarget: false,
                _logging));

            Steps.Add(new MoveFile(sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SqlMultiReaderConnector.dll"),
                sitecoreDirectory.CombineFile("bin", "Ucommerce.SqlMultiReaderConnector.dll"),
                backupTarget: false,
                _logging));
        }

        protected override void LogStart()
        {
            _logging.Information<MoveUcommerceBinaries>("Moving Ucommerce binaries");
        }
    }
}
