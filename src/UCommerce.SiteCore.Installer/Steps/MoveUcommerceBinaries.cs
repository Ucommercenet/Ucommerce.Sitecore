using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
    public class MoveUcommerceBinaries : IStep
    {
        private readonly DirectoryInfo _basePath;
        private readonly IInstallerLoggingService _logging;
        private readonly DirectoryInfo _sitecoreBasePath;

        public MoveUcommerceBinaries(DirectoryInfo basePath, DirectoryInfo sitecoreBasePath, IInstallerLoggingService logging)
        {
            _basePath = basePath;
            _sitecoreBasePath = sitecoreBasePath;
            _logging = logging;
        }

        public async Task Run()
        {
            _logging.Information<MoveUcommerceBinaries>("Moving Ucommerce binaries");
            var installationSteps = new List<IStep>();

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            // Therefor any older version located in the bin folder needs to be removed.
            //_postInstallationSteps.Add(new DeleteFile("~/bin/Ucommerce.Sitecore.CommerceConnect.dll"));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Infrastructure.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Infrastructure.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Sitecore.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Sitecore.dll")),
                backupTarget: false,
                _logging));

            // Move the Commerce Connect dependend files to the Commerce Connect app location.
            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Sitecore.CommerceConnect.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName,
                    "sitecore modules",
                    "Shell",
                    "uCommerce",
                    "Apps",
                    "Sitecore Connect.disabled",
                    "Ucommerce.Sitecore.CommerceConnect.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Web.Api.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Web.Api.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.SystemHttp.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.SystemHttp.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.SystemWeb.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.SystemWeb.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Web.Shell.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Web.Shell.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Admin.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Admin.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Pipelines.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Pipelines.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Presentation.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Presentation.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.NHibernate.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.NHibernate.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Pipelines.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Pipelines.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Sitecore.Web.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Sitecore.Web.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Api.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Api.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.Search.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.Search.dll")),
                backupTarget: false,
                _logging));

            installationSteps.Add(new MoveFile(new FileInfo(Path.Combine(_basePath.FullName, "bin", "ucommerce", "Ucommerce.SqlMultiReaderConnector.dll")),
                new FileInfo(Path.Combine(_sitecoreBasePath.FullName, "bin", "Ucommerce.SqlMultiReaderConnector.dll")),
                backupTarget: false,
                _logging));

            foreach (var postInstallationStep in installationSteps)
            {
                postInstallationStep.Run();
            }
        }
    }
}
