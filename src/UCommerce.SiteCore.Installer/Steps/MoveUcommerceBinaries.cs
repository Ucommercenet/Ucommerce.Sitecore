using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.FileExtensions;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveUcommerceBinaries : IStep
    {
        private readonly IInstallerLoggingService _logging;
        private readonly DirectoryInfo _sitecoreDirectory;

        public MoveUcommerceBinaries(DirectoryInfo sitecoreDirectory, IInstallerLoggingService logging)
        {
            _sitecoreDirectory = sitecoreDirectory;
            _logging = logging;
        }

        public async Task Run()
        {
            _logging.Information<MoveUcommerceBinaries>("Moving Ucommerce binaries");
            var installationSteps = new List<IStep>();

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            // Therefor any older version located in the bin folder needs to be removed.
            //_postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Sitecore.CommerceConnect.dll"));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Infrastructure.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Infrastructure.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Sitecore.dll"),
                backupTarget: false,
                _logging));

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.CommerceConnect.dll"),
                _sitecoreDirectory.CombineFile("sitecore modules",
                    "Shell",
                    "uCommerce",
                    "Apps",
                    "Sitecore Connect.disabled",
                    "Ucommerce.Sitecore.CommerceConnect.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Web.Api.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Web.Api.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SystemHttp.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.SystemHttp.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SystemWeb.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.SystemWeb.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Web.Shell.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Web.Shell.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Admin.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Admin.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Pipelines.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Pipelines.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Presentation.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Presentation.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.NHibernate.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.NHibernate.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Pipelines.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Pipelines.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Sitecore.Web.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Sitecore.Web.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Api.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Api.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.Search.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.Search.dll"),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(_sitecoreDirectory.CombineFile("bin", "ucommerce", "Ucommerce.SqlMultiReaderConnector.dll"),
                _sitecoreDirectory.CombineFile("bin", "Ucommerce.SqlMultiReaderConnector.dll"),
                backupTarget: false,
                _logging));

            foreach (var step in installationSteps)
            {
                await step.Run();
            }
        }
    }
}
